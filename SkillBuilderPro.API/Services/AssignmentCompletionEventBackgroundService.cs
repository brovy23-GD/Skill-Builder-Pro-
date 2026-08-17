using Microsoft.Extensions.Options;

namespace SkillBuilderPro.API.Services;

public sealed class AssignmentCompletionEventBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly AssignmentCompletionProcessingOptions _options;
    private readonly ILogger<AssignmentCompletionEventBackgroundService> _logger;

    public AssignmentCompletionEventBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<AssignmentCompletionProcessingOptions> options,
        ILogger<AssignmentCompletionEventBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var pollingInterval = TimeSpan.FromSeconds(Math.Max(1, _options.PollingSeconds));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var processor = scope.ServiceProvider.GetRequiredService<IAssignmentCompletionEventProcessor>();
                await processor.ProcessPendingBatchAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    "Assignment completion processing cycle failed with {ExceptionType}; processing will retry after the polling delay.",
                    exception.GetType().Name);
            }

            await Task.Delay(pollingInterval, stoppingToken);
        }
    }
}
