using System.Data;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.DrillImport;

public sealed class DrillImportService(AppDbContext db, ILogger<DrillImportService> logger)
{
    public async Task<DrillImportResult> RunAsync(
        string sourcePath,
        string expectedHash,
        bool dryRun,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(sourcePath))
            return Failure(dryRun, string.Empty, $"Source file does not exist: {sourcePath}");

        var (actualHash, sourceRows) = await DrillImportValidation.ReadAsync(sourcePath, cancellationToken);
        var validation = DrillImportValidation.Validate(sourceRows, actualHash, expectedHash);
        var before = await CaptureBaselineAsync(cancellationToken);

        if (validation.Errors.Count > 0)
        {
            logger.LogError(
                "Drill import validation failed for source hash {SourceHash} with {ErrorCount} errors.",
                actualHash,
                validation.Errors.Count);
            return BuildResult(
                success: false,
                dryRun,
                actualHash,
                sourceRows.Count,
                validation.Rows.Count,
                validation.Errors.Count,
                0, 0, 0, 0, 0, 0,
                validation.Rows.GroupBy(row => row.ImportKey).Count(group => group.Count() > 1),
                validation.Warnings.Count,
                0,
                false,
                before,
                before,
                validation.Errors,
                validation.Warnings);
        }

        var existing = await db.Drills.AsNoTracking().ToListAsync(cancellationToken);
        var plan = CreatePlan(validation.Rows, existing);

        if (dryRun)
        {
            logger.LogInformation(
                "Drill import dry run passed. Source={SourceCount}; insert={Insert}; update={Update}; unchanged={Unchanged}; video warnings={VideoWarnings}.",
                sourceRows.Count,
                plan.Inserts.Count,
                plan.Updates.Count,
                plan.Unchanged,
                validation.Warnings.Count);

            return await BuildVerifiedResultAsync(
                success: true,
                dryRun: true,
                actualHash,
                sourceRows.Count,
                validation.Rows.Count,
                invalidCount: 0,
                inserted: 0,
                updated: 0,
                unchanged: 0,
                wouldInsert: plan.Inserts.Count,
                wouldUpdate: plan.Updates.Count,
                wouldRemain: plan.Unchanged,
                duplicateImportKeys: 0,
                videoWarnings: validation.Warnings.Count,
                legacyMatchesAttached: plan.LegacyMatchesAttached,
                transactionCommitted: false,
                before,
                before,
                validation.Errors,
                validation.Warnings,
                cancellationToken);
        }

        ImportPlan? committedPlan = null;
        var strategy = db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            db.ChangeTracker.Clear();
            await using var transaction = await db.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);
            try
            {
                var trackedExisting = await db.Drills.ToListAsync(cancellationToken);
                var executionPlan = CreatePlan(validation.Rows, trackedExisting);

                foreach (var insertion in executionPlan.Inserts)
                    db.Drills.Add(MapNew(insertion));

                foreach (var update in executionPlan.Updates)
                    Apply(update.Source, update.Target);

                await db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                committedPlan = executionPlan;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                db.ChangeTracker.Clear();
                throw;
            }
        });

        var finalPlan = committedPlan
            ?? throw new InvalidOperationException("The drill import execution strategy completed without a committed plan.");
        db.ChangeTracker.Clear();
        var after = await CaptureBaselineAsync(cancellationToken);
        logger.LogInformation(
            "Drill import committed. Hash={SourceHash}; inserted={Inserted}; updated={Updated}; unchanged={Unchanged}; video warnings={VideoWarnings}.",
            actualHash,
            finalPlan.Inserts.Count,
            finalPlan.Updates.Count,
            finalPlan.Unchanged,
            validation.Warnings.Count);

        return await BuildVerifiedResultAsync(
            success: true,
            dryRun: false,
            actualHash,
            sourceRows.Count,
            validation.Rows.Count,
            invalidCount: 0,
            inserted: finalPlan.Inserts.Count,
            updated: finalPlan.Updates.Count,
            unchanged: finalPlan.Unchanged,
            wouldInsert: 0,
            wouldUpdate: 0,
            wouldRemain: 0,
            duplicateImportKeys: 0,
            videoWarnings: validation.Warnings.Count,
            legacyMatchesAttached: finalPlan.LegacyMatchesAttached,
            transactionCommitted: true,
            before,
            after,
            validation.Errors,
            validation.Warnings,
            cancellationToken);
    }

    private ImportPlan CreatePlan(
        IReadOnlyList<ValidatedDrillImportRow> sourceRows,
        IReadOnlyList<Drill> existing)
    {
        var byImportKey = existing
            .Where(drill => !string.IsNullOrWhiteSpace(drill.ExternalSourceKey))
            .ToDictionary(drill => drill.ExternalSourceKey!, StringComparer.Ordinal);
        var unowned = existing.Where(drill => string.IsNullOrWhiteSpace(drill.ExternalSourceKey)).ToArray();
        var claimedLegacyIds = new HashSet<int>();
        var inserts = new List<ValidatedDrillImportRow>();
        var updates = new List<ImportUpdate>();
        var unchanged = 0;
        var legacyMatchesAttached = 0;

        foreach (var source in sourceRows)
        {
            if (byImportKey.TryGetValue(source.ImportKey, out var owned))
            {
                if (Equivalent(source, owned)) unchanged++;
                else updates.Add(new ImportUpdate(source, owned));
                continue;
            }

            var legacyMatches = unowned
                .Where(candidate => !claimedLegacyIds.Contains(candidate.Id) && ReliableLegacyMatch(source, candidate))
                .ToArray();
            if (legacyMatches.Length == 1)
            {
                var legacy = legacyMatches[0];
                claimedLegacyIds.Add(legacy.Id);
                updates.Add(new ImportUpdate(source, legacy));
                legacyMatchesAttached++;
            }
            else
            {
                inserts.Add(source);
            }
        }

        return new ImportPlan(inserts, updates, unchanged, legacyMatchesAttached);
    }

    private static bool ReliableLegacyMatch(ValidatedDrillImportRow source, Drill candidate)
    {
        if (source.YouTubeVideoId is null
            || !DrillImportValidation.TryGetYouTubeVideoId(candidate.VideoUrl, out var candidateVideoId))
            return false;

        return string.Equals(source.YouTubeVideoId, candidateVideoId, StringComparison.Ordinal)
            && Same(source.Name, candidate.Name)
            && Same(source.Sport, candidate.Sport)
            && Same(source.Category, candidate.Category)
            && Same(source.SubCategory, candidate.SubCategory);
    }

    private static bool Equivalent(ValidatedDrillImportRow source, Drill target) =>
        string.Equals(source.ImportKey, target.ExternalSourceKey, StringComparison.Ordinal)
        && Exact(source.Name, target.Name)
        && Exact(source.Sport, target.Sport)
        && Exact(source.Category, target.Category)
        && Exact(source.SubCategory, target.SubCategory)
        && Exact(source.Description, target.Description)
        && source.Difficulty == target.Difficulty
        && Exact(source.Duration, target.Duration)
        && Exact(source.VideoUrl, target.VideoUrl)
        && source.DateCreated == target.DateCreated;

    private static Drill MapNew(ValidatedDrillImportRow source)
    {
        var drill = new Drill();
        Apply(source, drill);
        return drill;
    }

    private static void Apply(ValidatedDrillImportRow source, Drill target)
    {
        target.ExternalSourceKey = source.ImportKey;
        target.Name = source.Name;
        target.Sport = source.Sport;
        target.Category = source.Category;
        target.SubCategory = source.SubCategory;
        target.Description = source.Description;
        target.Difficulty = source.Difficulty;
        target.Duration = source.Duration;
        target.VideoUrl = source.VideoUrl;
        target.DateCreated = source.DateCreated;
    }

    private async Task<DrillImportResult> BuildVerifiedResultAsync(
        bool success,
        bool dryRun,
        string sourceHash,
        int sourceCount,
        int validCount,
        int invalidCount,
        int inserted,
        int updated,
        int unchanged,
        int wouldInsert,
        int wouldUpdate,
        int wouldRemain,
        int duplicateImportKeys,
        int videoWarnings,
        int legacyMatchesAttached,
        bool transactionCommitted,
        DrillDataBaseline before,
        DrillDataBaseline after,
        IReadOnlyList<string> errors,
        IReadOnlyList<string> warnings,
        CancellationToken cancellationToken)
    {
        var total = await db.Drills.AsNoTracking().CountAsync(cancellationToken);
        var ownedQuery = db.Drills.AsNoTracking()
            .Where(drill => drill.ExternalSourceKey != null
                && drill.ExternalSourceKey.StartsWith(DrillImportValidation.DatasetVersion + ":"));
        var owned = await ownedQuery.CountAsync(cancellationToken);
        var distribution = await ownedQuery.GroupBy(drill => drill.Sport)
            .Select(group => new { Sport = group.Key, Count = group.Count() })
            .ToDictionaryAsync(item => item.Sport, item => item.Count, cancellationToken);
        var groups = await ownedQuery.GroupBy(drill => new { drill.Sport, drill.Category, drill.SubCategory })
            .Select(group => group.Count()).ToListAsync(cancellationToken);

        return new DrillImportResult(
            success,
            dryRun,
            sourceHash,
            sourceCount,
            validCount,
            invalidCount,
            inserted,
            updated,
            unchanged,
            wouldInsert,
            wouldUpdate,
            wouldRemain,
            duplicateImportKeys,
            videoWarnings,
            legacyMatchesAttached,
            transactionCommitted,
            total,
            owned,
            total - owned,
            distribution,
            groups.Count,
            groups.Count(count => count != 5),
            before,
            after,
            before == after,
            errors,
            warnings);
    }

    private async Task<DrillDataBaseline> CaptureBaselineAsync(CancellationToken cancellationToken) =>
        new(
            await db.Users.CountAsync(cancellationToken),
            await db.UserProfiles.CountAsync(cancellationToken),
            await db.AthleteGoals.CountAsync(cancellationToken),
            await db.DrillAssignments.CountAsync(cancellationToken),
            await db.DrillAssignmentRecipients.CountAsync(cancellationToken),
            await db.ProgressLogs.CountAsync(cancellationToken),
            await db.Schedules.CountAsync(cancellationToken),
            await db.AthleteProgressions.CountAsync(cancellationToken),
            await db.AthleteSkillProgress.CountAsync(cancellationToken),
            await db.AthleteRankHistories.CountAsync(cancellationToken),
            await db.AthleteSkillLevelHistories.CountAsync(cancellationToken),
            await db.AthleteAchievements.CountAsync(cancellationToken),
            await db.TrainingRequests.CountAsync(cancellationToken),
            await db.Notifications.CountAsync(cancellationToken),
            await db.NotificationEvents.CountAsync(cancellationToken));

    private static DrillImportResult BuildResult(
        bool success, bool dryRun, string sourceHash, int sourceCount, int validCount, int invalidCount,
        int inserted, int updated, int unchanged, int wouldInsert, int wouldUpdate, int wouldRemain,
        int duplicateImportKeys, int videoWarnings, int legacyMatchesAttached, bool transactionCommitted,
        DrillDataBaseline before, DrillDataBaseline after, IReadOnlyList<string> errors, IReadOnlyList<string> warnings) =>
        new(success, dryRun, sourceHash, sourceCount, validCount, invalidCount, inserted, updated, unchanged,
            wouldInsert, wouldUpdate, wouldRemain, duplicateImportKeys, videoWarnings, legacyMatchesAttached,
            transactionCommitted, 0, 0, 0, new Dictionary<string, int>(), 0, 0, before, after,
            before == after, errors, warnings);

    private static DrillImportResult Failure(bool dryRun, string hash, string error)
    {
        var empty = new DrillDataBaseline(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        return BuildResult(false, dryRun, hash, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, false,
            empty, empty, [error], []);
    }

    private static bool Same(string? left, string? right) =>
        string.Equals(Normalize(left), Normalize(right), StringComparison.Ordinal);

    private static string Normalize(string? value) =>
        string.Join(' ', (value ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .ToUpperInvariant();

    private static bool Exact(string? left, string? right) =>
        string.Equals(left ?? string.Empty, right ?? string.Empty, StringComparison.Ordinal);

    private sealed record ImportUpdate(ValidatedDrillImportRow Source, Drill Target);
    private sealed record ImportPlan(
        IReadOnlyList<ValidatedDrillImportRow> Inserts,
        IReadOnlyList<ImportUpdate> Updates,
        int Unchanged,
        int LegacyMatchesAttached);
}
