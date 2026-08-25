using System.Security.Cryptography;
using System.Text.Json;

namespace SkillBuilderPro.API.DrillImport;

public static class DrillImportValidation
{
    public const string DatasetVersion = "skillbuilderpro-900-v1";

    private static readonly IReadOnlyDictionary<string, string> SupportedSports =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["BASEBALL"] = "Baseball",
            ["BASKETBALL"] = "Basketball",
            ["FOOTBALL"] = "Football",
            ["HOCKEY"] = "Hockey",
            ["SOCCER"] = "Soccer",
            ["SOFTBALL"] = "Softball"
        };

    public static string CreateImportKey(int sourceId) =>
        $"{DatasetVersion}:{sourceId}";

    public static async Task<(string Hash, IReadOnlyList<DrillImportSourceRow> Rows)> ReadAsync(
        string sourcePath,
        CancellationToken cancellationToken = default)
    {
        await using var hashStream = File.OpenRead(sourcePath);
        var hashBytes = await SHA256.HashDataAsync(hashStream, cancellationToken);
        var hash = Convert.ToHexString(hashBytes);

        await using var jsonStream = File.OpenRead(sourcePath);
        var rows = await JsonSerializer.DeserializeAsync<List<DrillImportSourceRow>>(
            jsonStream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken) ?? [];

        return (hash, rows);
    }

    public static (IReadOnlyList<ValidatedDrillImportRow> Rows, IReadOnlyList<string> Errors, IReadOnlyList<string> Warnings)
        Validate(
            IReadOnlyList<DrillImportSourceRow> sourceRows,
            string actualHash,
            string expectedHash)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        if (!string.Equals(actualHash, expectedHash.Trim(), StringComparison.OrdinalIgnoreCase))
            errors.Add($"Source SHA-256 mismatch. Expected {expectedHash.Trim().ToUpperInvariant()}, actual {actualHash}.");

        if (sourceRows.Count != 900)
            errors.Add($"Expected exactly 900 source rows; found {sourceRows.Count}.");

        var duplicateSourceIds = sourceRows.GroupBy(row => row.Id).Where(group => group.Count() > 1).ToArray();
        if (duplicateSourceIds.Length > 0)
            errors.Add($"Duplicate source IDs found: {string.Join(", ", duplicateSourceIds.Select(group => group.Key))}.");

        var validated = new List<ValidatedDrillImportRow>(sourceRows.Count);
        foreach (var row in sourceRows)
        {
            var prefix = $"Source row {row.Id}";
            if (row.Id <= 0) errors.Add($"{prefix}: source ID must be positive.");
            if (string.IsNullOrWhiteSpace(row.Name)) errors.Add($"{prefix}: name is required.");
            if (string.IsNullOrWhiteSpace(row.Category)) errors.Add($"{prefix}: category is required.");
            if (string.IsNullOrWhiteSpace(row.SubCategory)) errors.Add($"{prefix}: subCategory is required.");
            if (string.IsNullOrWhiteSpace(row.Description)) errors.Add($"{prefix}: description is required.");
            if (string.IsNullOrWhiteSpace(row.Duration)) errors.Add($"{prefix}: duration is required.");
            if (string.IsNullOrWhiteSpace(row.VideoUrl)) errors.Add($"{prefix}: videoUrl is required.");
            if (!SupportedSports.TryGetValue(row.Sport.Trim(), out var sport))
            {
                errors.Add($"{prefix}: unsupported sport '{row.Sport}'.");
                continue;
            }

            if (row.Difficulty is < 1 or > 5)
                errors.Add($"{prefix}: difficulty {row.Difficulty} is outside 1-5.");

            if (!TryValidateDuration(row.Duration, out var normalizedDuration))
                errors.Add($"{prefix}: duration '{row.Duration}' is not a supported m:ss or h:mm:ss value.");

            var playable = TryGetYouTubeVideoId(row.VideoUrl, out var videoId);
            if (!playable)
                warnings.Add($"{prefix}: video URL is retained but is not in a syntactically supported YouTube format.");

            validated.Add(new ValidatedDrillImportRow(
                row.Id,
                CreateImportKey(row.Id),
                row.Name.Trim(),
                sport,
                row.Category.Trim(),
                row.SubCategory.Trim(),
                row.Description.Trim(),
                row.Difficulty,
                normalizedDuration ?? row.Duration.Trim(),
                row.VideoUrl.Trim(),
                row.DateCreated,
                playable ? videoId : null));
        }

        var duplicateKeys = validated.GroupBy(row => row.ImportKey, StringComparer.Ordinal)
            .Where(group => group.Count() > 1).ToArray();
        if (duplicateKeys.Length > 0)
            errors.Add($"Duplicate ImportKeys found: {string.Join(", ", duplicateKeys.Select(group => group.Key))}.");

        var sports = validated.GroupBy(row => row.Sport, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);
        if (sports.Count != 6)
            errors.Add($"Expected exactly 6 supported sports; found {sports.Count}.");
        foreach (var sport in SupportedSports.Values)
        {
            var count = sports.GetValueOrDefault(sport);
            if (count != 150) errors.Add($"Expected 150 {sport} rows; found {count}.");
        }

        var groups = validated.GroupBy(
            row => $"{row.Sport}\u001f{row.Category}\u001f{row.SubCategory}",
            StringComparer.OrdinalIgnoreCase).ToArray();
        if (groups.Length != 180)
            errors.Add($"Expected exactly 180 sport/category/subcategory groups; found {groups.Length}.");
        var invalidGroups = groups.Where(group => group.Count() != 5).ToArray();
        if (invalidGroups.Length > 0)
            errors.Add($"Expected exactly 5 rows per group; {invalidGroups.Length} groups differ.");

        var uniqueVideoUrls = validated.Select(row => row.VideoUrl)
            .Distinct(StringComparer.OrdinalIgnoreCase).Count();
        if (uniqueVideoUrls != 900)
            errors.Add($"Expected 900 unique video URLs; found {uniqueVideoUrls}.");

        return (validated, errors, warnings);
    }

    public static bool TryGetYouTubeVideoId(string? value, out string videoId)
    {
        videoId = string.Empty;
        if (!Uri.TryCreate(value?.Trim(), UriKind.Absolute, out var uri)
            || uri.Scheme != Uri.UriSchemeHttps)
            return false;

        var host = uri.Host.ToLowerInvariant();
        if (host.StartsWith("www.", StringComparison.Ordinal)) host = host[4..];

        string? candidate = null;
        if (host == "youtu.be")
        {
            candidate = uri.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        }
        else if (host is "youtube.com" or "m.youtube.com" or "youtube-nocookie.com")
        {
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 2 && segments[0] is "embed" or "shorts" or "live")
                candidate = segments[1];
            else if (uri.AbsolutePath.Equals("/watch", StringComparison.OrdinalIgnoreCase))
                candidate = ParseQuery(uri.Query, "v");
        }

        if (candidate?.Length != 11
            || candidate.Any(character => !(char.IsLetterOrDigit(character) || character is '-' or '_')))
            return false;

        videoId = candidate;
        return true;
    }

    private static bool TryValidateDuration(string value, out string? normalized)
    {
        normalized = value.Trim();
        var parts = normalized.Split(':');
        if (parts.Length is < 2 or > 3 || parts.Any(part => !int.TryParse(part, out _))) return false;
        var numbers = parts.Select(int.Parse).ToArray();
        if (numbers.Any(number => number < 0)) return false;
        if (numbers[^1] > 59) return false;
        if (parts.Length == 3 && numbers[^2] > 59) return false;
        return true;
    }

    private static string? ParseQuery(string query, string key)
    {
        foreach (var pair in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = pair.Split('=', 2);
            if (parts.Length == 2
                && Uri.UnescapeDataString(parts[0]).Equals(key, StringComparison.OrdinalIgnoreCase))
                return Uri.UnescapeDataString(parts[1]);
        }
        return null;
    }
}
