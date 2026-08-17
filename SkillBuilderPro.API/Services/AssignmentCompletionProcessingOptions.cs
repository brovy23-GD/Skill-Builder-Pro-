namespace SkillBuilderPro.API.Services;

public sealed class AssignmentCompletionProcessingOptions
{
    public const string SectionName = "AssignmentCompletionProcessing";
    public int PollingSeconds { get; set; } = 10;
    public int BatchSize { get; set; } = 20;
    public int MaxAttempts { get; set; } = 5;
}
