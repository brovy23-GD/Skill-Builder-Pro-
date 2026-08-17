namespace SkillBuilderPro.API.Services;

public interface IAssignmentCompletionEventProcessor
{
    Task<int> ProcessPendingBatchAsync(CancellationToken cancellationToken = default);
}
