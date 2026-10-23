using Eras.Application.Contracts.Persistence;
using Eras.Application.Services;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Jobs;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.Jobs;


using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class EvaluationStatusSyncJobTests
{
    private sealed class TestableEvaluationStatusSyncJob : EvaluationStatusSyncJob
    {
        public TestableEvaluationStatusSyncJob(IServiceScopeFactory scopeFactory, ILogger<EvaluationStatusSyncJob> logger, TimeSpan timespan)
            : base(scopeFactory, logger, timespan){ }

        public TestableEvaluationStatusSyncJob(IServiceScopeFactory scopeFactory, ILogger<EvaluationStatusSyncJob> logger)
            : base(scopeFactory, logger) { }

        public Task ExecuteAsyncPublic(CancellationToken cancellationToken)
            => ExecuteAsync(cancellationToken);

        public Task RunAsyncPublic(CancellationToken cancellationToken) => RunAsync(cancellationToken);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEvaluationExists_UpdatesEvaluation()
    {
        var evaluation = new Evaluation { Id = 123 };

        var repository = new Mock<IEvaluationRepository>();

        repository
            .Setup(x => x.GetExpiredWithPendingStatusAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<DateTime>(), CancellationToken.None))
            .ReturnsAsync(new[] { evaluation });

        var updater = new Mock<IEvaluationStatusUpdater>();

        updater
            .Setup(x => x.UpdateStatusAsync(evaluation))
            .Returns(Task.CompletedTask);

        var services = new ServiceCollection();

        services.AddScoped<IEvaluationRepository>(_ => repository.Object);
        services.AddScoped<IEvaluationStatusUpdater>(_ => updater.Object);

        await using var provider = services.BuildServiceProvider();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
        var logger = new Mock<ILogger<EvaluationStatusSyncJob>>();
        var sut = new TestableEvaluationStatusSyncJob(scopeFactory, logger.Object, TimeSpan.Zero);

        using var cts = new CancellationTokenSource();

        repository
            .Setup(x => x.GetExpiredWithPendingStatusAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<DateTime>(), CancellationToken.None))
            .ReturnsAsync(new[] { evaluation })
            .Callback(() => cts.Cancel());

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => sut.ExecuteAsyncPublic(cts.Token));
    }

    [Fact]
    public async Task RunAsync_WhenEvaluationExists_UpdatesEvaluation()
    {
        // Arrange
        var evaluation = new Evaluation { Id = 123 };

        var repository = new Mock<IEvaluationRepository>();

        repository
            .Setup(x => x.GetExpiredWithPendingStatusAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<DateTime>(), CancellationToken.None))
            .ReturnsAsync(new[] { evaluation });

        var updater = new Mock<IEvaluationStatusUpdater>();

        updater
            .Setup(x => x.UpdateStatusAsync(evaluation))
            .Returns(Task.CompletedTask);

        var services = new ServiceCollection();

        services.AddScoped<IEvaluationRepository>(_ => repository.Object);
        services.AddScoped<IEvaluationStatusUpdater>(_ => updater.Object);

        await using var provider = services.BuildServiceProvider();

        var logger = new Mock<ILogger<EvaluationStatusSyncJob>>();

        var sut = new TestableEvaluationStatusSyncJob(
            provider.GetRequiredService<IServiceScopeFactory>(),
            logger.Object);

        // Act
        await sut.RunAsyncPublic(CancellationToken.None);

        // Assert
        updater.Verify(
            x => x.UpdateStatusAsync(evaluation),
            Times.Once);
    }

    [Fact]
    public async Task RunAsync_WhenNoExpiredEvaluations_DoesNotUpdate()
    {
        // Arrange
        var repository = new Mock<IEvaluationRepository>();

        repository
            .Setup(x => x.GetExpiredWithPendingStatusAsync(
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<DateTime>(), CancellationToken.None))
            .ReturnsAsync(Array.Empty<Evaluation>());

        var updater = new Mock<IEvaluationStatusUpdater>();

        var services = new ServiceCollection();

        services.AddScoped<IEvaluationRepository>(_ => repository.Object);
        services.AddScoped<IEvaluationStatusUpdater>(_ => updater.Object);

        await using var provider = services.BuildServiceProvider();

        var logger = new Mock<ILogger<EvaluationStatusSyncJob>>();

        var sut = new TestableEvaluationStatusSyncJob(
            provider.GetRequiredService<IServiceScopeFactory>(),
            logger.Object);

        // Act
        await sut.RunAsyncPublic(CancellationToken.None);

        // Assert
        updater.Verify(
            x => x.UpdateStatusAsync(It.IsAny<Evaluation>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCancellationIsRequested_StopsExecution()
    {
        // Arrange
        var repository = new Mock<IEvaluationRepository>();
        using var cts = new CancellationTokenSource();

        repository
            .Setup(x => x.GetExpiredWithPendingStatusAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Evaluation>())
            .Callback(() => cts.Cancel());

        var updater = new Mock<IEvaluationStatusUpdater>();
        var services = new ServiceCollection();
        services.AddScoped<IEvaluationRepository>(_ => repository.Object);
        services.AddScoped<IEvaluationStatusUpdater>(_ => updater.Object);

        await using var provider = services.BuildServiceProvider();
        var logger = new Mock<ILogger<EvaluationStatusSyncJob>>();

        var sut = new TestableEvaluationStatusSyncJob(
            provider.GetRequiredService<IServiceScopeFactory>(),
            logger.Object,
            TimeSpan.FromMilliseconds(1));

        // Act
        await sut.ExecuteAsyncPublic(cts.Token);

        // Assert
        updater.Verify(x => x.UpdateStatusAsync(It.IsAny<Evaluation>()),
            Times.Never);
    }
}