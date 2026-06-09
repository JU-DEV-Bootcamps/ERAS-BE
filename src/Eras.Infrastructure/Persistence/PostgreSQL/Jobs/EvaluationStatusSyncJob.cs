using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);

    public EvaluationStatusSyncJob(IServiceScopeFactory scopeFactory,
       ILogger<EvaluationStatusSyncJob> logger)
   {
       _scopeFactory = scopeFactory;
       _logger = logger;
   }

   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
       while (!stoppingToken.IsCancellationRequested)
       {
           await Task.Delay(_interval, stoppingToken);
           await RunAsync(stoppingToken);
       }
   }

   private async Task RunAsync(CancellationToken ct)
   {
       await using var scope = _scopeFactory.CreateAsyncScope();
       var repository = scope.ServiceProvider.GetRequiredService<IEvaluationRepository>();
       var updater = scope.ServiceProvider.GetRequiredService<EvaluationStatusUpdater>();

       var expiredStatuses = new[]
       {
           EvaluationStatus.Ready.ToString(),
           EvaluationStatus.InProgress.ToString()
       };

       var evaluations = await repository.GetExpiredWithPendingStatusAsync(
           expiredStatuses, DateTime.UtcNow);

       foreach (var evaluation in evaluations)
       {
           await updater.UpdateStatusAsync(evaluation);
           _logger.LogInformation(
               "Evaluation {Id} status synced to {Status}", evaluation.Id, evaluation.Status);
       }
   }
}
