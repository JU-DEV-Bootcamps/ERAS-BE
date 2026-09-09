using System.Diagnostics;
using Eras.Application.Contracts.Services;
using Eras.Application.Models;
using Eras.Infrastructure.BackgroundProcessing;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace Eras.Infrastructure.Tests.BackgroundProcessing;

public sealed class AttachmentRelocationJobTests
{
    private static IOptions<FileStorageSettings> MakeSettings(double IntervalMinutes) =>
        Options.Create(new FileStorageSettings
        {
            BasePath = "unused",
            AllowedExtensions = Array.Empty<string>(),
            StorageRelocationIntervalMinutes = IntervalMinutes == 0 ? 0 : (int)IntervalMinutes,
        });

    private static IServiceScopeFactory BuildScopeFactory(IAttachmentRelocationService RelocationService)
    {
        var services = new ServiceCollection();
        services.AddSingleton(RelocationService);
        ServiceProvider provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task ExecuteAsync_Should_CallRelocationServiceRunAsync_AtLeastOnceAsync()
    {
        var relocationService = new Mock<IAttachmentRelocationService>();
        relocationService
            .Setup(R => R.RunAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        IServiceScopeFactory scopeFactory = BuildScopeFactory(relocationService.Object);

        var job = new AttachmentRelocationJob(
            scopeFactory,
            MakeSettings(1),
            Mock.Of<ILogger<AttachmentRelocationJob>>());

        await job.StartAsync(CancellationToken.None);
        await Task.Delay(50);
        await job.StopAsync(CancellationToken.None);

        relocationService.Verify(R => R.RunAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExecuteAsync_Should_LogError_AndKeepRunning_WhenRelocationServiceThrowsAsync()
    {
        var relocationService = new Mock<IAttachmentRelocationService>();
        relocationService
            .Setup(R => R.RunAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        IServiceScopeFactory scopeFactory = BuildScopeFactory(relocationService.Object);
        var logger = new Mock<ILogger<AttachmentRelocationJob>>();

        var job = new AttachmentRelocationJob(scopeFactory, MakeSettings(1), logger.Object);

        await job.StartAsync(CancellationToken.None);
        await Task.Delay(50);
        await job.StopAsync(CancellationToken.None);

        logger.Verify(
            L => L.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<InvalidOperationException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task StopAsync_Should_CompletePromptly_WhenCancelledDuringDelayAsync()
    {
        var relocationService = new Mock<IAttachmentRelocationService>();
        relocationService
            .Setup(R => R.RunAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        IServiceScopeFactory scopeFactory = BuildScopeFactory(relocationService.Object);

        var job = new AttachmentRelocationJob(
            scopeFactory,
            MakeSettings(60),
            Mock.Of<ILogger<AttachmentRelocationJob>>());

        await job.StartAsync(CancellationToken.None);
        await Task.Delay(20);

        var stopwatch = Stopwatch.StartNew();
        await job.StopAsync(CancellationToken.None);
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < 5000);
    }

    [Fact]
    public async Task ExecuteAsync_Should_CreateAndDisposeAScope_ForEachTickAsync()
    {
        var relocationService = new Mock<IAttachmentRelocationService>();
        relocationService
            .Setup(R => R.RunAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var services = new ServiceCollection();
        services.AddScoped(_ => relocationService.Object);
        ServiceProvider provider = services.BuildServiceProvider();
        IServiceScopeFactory scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        var job = new AttachmentRelocationJob(
            scopeFactory,
            MakeSettings(1),
            Mock.Of<ILogger<AttachmentRelocationJob>>());

        await job.StartAsync(CancellationToken.None);
        await Task.Delay(50);
        await job.StopAsync(CancellationToken.None);

        relocationService.Verify(R => R.RunAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }
}
