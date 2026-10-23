using Eras.Application.Contracts.Persistence;
using Eras.Application.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using static Eras.Domain.Entities.EvaluationConstants;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Jobs;

public class EvaluationStatusSyncJob : BackgroundService
{
   private readonly IServiceScopeFactory _scopeFactory;
   private readonly ILogger<EvaluationStatusSyncJob> _logger;
    private readonly TimeSpan _interval;

    public EvaluationStatusSyncJob(IServiceScopeFactory scopeFactory,
       ILogger<EvaluationStatusSyncJob> logger, TimeSpan? interval = null)
   {
       _scopeFactory = scopeFactory;
       _logger = logger;
       _interval = interval ?? TimeSpan.FromHours(2);
   }

   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
       while (!stoppingToken.IsCancellationRequested)
       {
            await RunAsync(stoppingToken);
            if (stoppingToken.IsCancellationRequested) { break; }
            await Task.Delay(_interval, stoppingToken);
        }
    }

   protected async Task RunAsync(CancellationToken ct)
   {
       await using var scope = _scopeFactory.CreateAsyncScope();
       var repository = scope.ServiceProvider.GetRequiredService<IEvaluationRepository>();
       var updater = scope.ServiceProvider.GetRequiredService<IEvaluationStatusUpdater>();

       var expiredStatuses = new[]
       {
           EvaluationStatus.Ready.ToString(),
           EvaluationStatus.InProgress.ToString()
       };

       var evaluations = await repository.GetExpiredWithPendingStatusAsync(
           expiredStatuses, DateTime.UtcNow, new CancellationToken());

       foreach (var evaluation in evaluations)
       {
            ct.ThrowIfCancellationRequested();
           await updater.UpdateStatusAsync(evaluation);
           _logger.LogInformation(
               "Evaluation {Id} status synced to {Status}", evaluation.Id, evaluation.Status);
       }
   }
}
