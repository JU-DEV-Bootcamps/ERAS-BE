
using Eras.Application.Contracts.Services;
using Eras.Application.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eras.Infrastructure.BackgroundProcessing;

public sealed class AttachmentRelocationJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AttachmentRelocationJob> _logger;
    private readonly TimeSpan _interval;

    public AttachmentRelocationJob(IServiceScopeFactory ScopeFactory,
        IOptions<FileStorageSettings> Settings, ILogger<AttachmentRelocationJob> Logger)
    {
        _scopeFactory = ScopeFactory;
        _logger = Logger;
        _interval = TimeSpan.FromMinutes(Settings.Value.StorageRelocationIntervalMinutes);
    }

    protected override async Task ExecuteAsync(CancellationToken StoppingToken)
    {
        while (!StoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunAsync(StoppingToken);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Storage relocation sweep failed.");
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
        var relocationService = scope.ServiceProvider.GetRequiredService<IAttachmentRelocationService>();
        await relocationService.RunAsync(CancellationToken);
    }
}
