using Eras.Application.Contracts.Services;
using Eras.Application.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Jobs;

/// <summary>
/// Runs at boot and then on a configurable interval (default hourly — see
/// <see cref="FileStorageSettings.TempAttachmentCleanupIntervalHours"/>), sweeping expired "Temp"
/// (draft-session-staged, never-claimed) attachments — e.g. left behind when a user closes a
/// create/edit form without saving. The actual sweep logic lives in
/// <see cref="ITempAttachmentCleanupService"/>; this class only owns the schedule.
/// </summary>
public sealed class TempAttachmentCleanupJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TempAttachmentCleanupJob> _logger;
    private readonly TimeSpan _interval;

    public TempAttachmentCleanupJob(
        IServiceScopeFactory ScopeFactory,
        IOptions<FileStorageSettings> Settings,
        ILogger<TempAttachmentCleanupJob> Logger)
    {
        _scopeFactory = ScopeFactory;
        _logger = Logger;
        _interval = TimeSpan.FromHours(Settings.Value.TempAttachmentCleanupIntervalHours);
    }

    protected override async Task ExecuteAsync(CancellationToken StoppingToken)
    {
        while (!StoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunAsync(StoppingToken);
            }
            catch (Exception ex)
            {
                // A failed sweep never stops the host or the schedule — retried next interval.
                _logger.LogError(ex, "Temp attachment cleanup sweep failed.");
            }

            try
            {
                await Task.Delay(_interval, StoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task RunAsync(CancellationToken CancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var cleanupService = scope.ServiceProvider.GetRequiredService<ITempAttachmentCleanupService>();
        await cleanupService.RunAsync(CancellationToken);
    }
}
