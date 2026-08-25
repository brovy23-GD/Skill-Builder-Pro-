using System.Text.Json.Serialization;

namespace SkillBuilderPro.API.DrillImport;

public sealed class DrillImportSourceRow
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("sport")]
    public string Sport { get; init; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; init; } = string.Empty;

    [JsonPropertyName("subCategory")]
    public string SubCategory { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("difficulty")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Difficulty { get; init; }

    [JsonPropertyName("duration")]
    public string Duration { get; init; } = string.Empty;

    [JsonPropertyName("videoUrl")]
    public string VideoUrl { get; init; } = string.Empty;

    [JsonPropertyName("dateCreated")]
    public DateTime? DateCreated { get; init; }
}

public sealed record ValidatedDrillImportRow(
    int SourceId,
    string ImportKey,
    string Name,
    string Sport,
    string Category,
    string SubCategory,
    string Description,
    int Difficulty,
    string Duration,
    string VideoUrl,
    DateTime? DateCreated,
    string? YouTubeVideoId);

public sealed record DrillDataBaseline(
    int IdentityUsers,
    int UserProfiles,
    int Goals,
    int Assignments,
    int AssignmentRecipients,
    int ProgressLogs,
    int TrainingSchedules,
    int AthleteProgressions,
    int AthleteSkillProgress,
    int AthleteRankHistory,
    int AthleteSkillLevelHistory,
    int AthleteAchievements,
    int TrainingRequests,
    int Notifications,
    int NotificationEvents);

public sealed record DrillImportResult(
    bool Success,
    bool DryRun,
    string SourceHash,
    int SourceCount,
    int ValidCount,
    int InvalidCount,
    int Inserted,
    int Updated,
    int Unchanged,
    int WouldInsert,
    int WouldUpdate,
    int WouldRemainUnchanged,
    int DuplicateImportKeys,
    int VideoWarnings,
    int LegacyMatchesAttached,
    bool TransactionCommitted,
    int TotalDatabaseDrills,
    int ImportOwnedDrills,
    int LegacyDrills,
    IReadOnlyDictionary<string, int> ImportOwnedSportDistribution,
    int ImportOwnedGroupCount,
    int ImportOwnedGroupsNotFive,
    DrillDataBaseline BeforeBaseline,
    DrillDataBaseline AfterBaseline,
    bool UnrelatedDataPreserved,
    IReadOnlyList<string> Errors,
    IReadOnlyList<string> Warnings);
