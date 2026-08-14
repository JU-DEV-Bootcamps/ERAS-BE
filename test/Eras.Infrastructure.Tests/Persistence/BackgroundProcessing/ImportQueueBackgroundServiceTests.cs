using System.Text.Json;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.Configurations.Queries.GetConfiguration;
using Eras.Application.Models;
using Eras.Application.Models.Response.Common;
using Eras.Application.Services;
using Eras.Domain.Entities;
using Eras.Infrastructure.BackgroundProcessing;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Infrastructure.Tests.Persistence.BackgroundProcessing;

public class ImportQueueBackgroundServiceTests
{
    private readonly Mock<IServiceScopeFactory> _scopeFactory;
    private readonly Mock<IImportJobQueue> _queue;
    private readonly Mock<ILogger<ImportQueueBackgroundService>> _logger;
    private readonly ImportQueueBackgroundService _service;

    public ImportQueueBackgroundServiceTests()
    {
        _scopeFactory = new Mock<IServiceScopeFactory>();
        _queue = new Mock<IImportJobQueue>();
        _logger = new Mock<ILogger<ImportQueueBackgroundService>>();
        _service = new ImportQueueBackgroundService(_scopeFactory.Object, _queue.Object, _logger.Object);
    }

    private ImportQueueBackgroundService CreateService()
    {
        return new ImportQueueBackgroundService(
            _scopeFactory.Object,
            _queue.Object,
            _logger.Object);
    }

    private void SetupScope(
    Mock<IImportJobRepository>? jobRepository = null,
    Mock<IImportJobItemRepository>? itemRepository = null,
    Mock<IPollOrchestratorServiceV2>? orchestrator = null,
    Mock<IMediator>? mediator = null,
    Mock<ICosmicLatteAPIService>? cosmicLatte = null)
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var scope = new Mock<IServiceScope>();

        if (jobRepository != null)
        {
            serviceProvider
                .Setup(P => P.GetService(typeof(IImportJobRepository)))
                .Returns(jobRepository.Object);
        }

        if (itemRepository != null)
        {
            serviceProvider
                .Setup(P => P.GetService(typeof(IImportJobItemRepository)))
                .Returns(itemRepository.Object);
        }

        if (mediator != null)
        {
            serviceProvider
                .Setup(P => P.GetService(typeof(IMediator)))
                .Returns(mediator.Object);
        }

        if (cosmicLatte != null)
        {
            serviceProvider
                .Setup(P => P.GetService(typeof(ICosmicLatteAPIService)))
                .Returns(cosmicLatte.Object);
        }

        if (orchestrator != null)
        {
            serviceProvider
                .Setup(P => P.GetService(typeof(IPollOrchestratorServiceV2)))
                .Returns(orchestrator.Object);
        }

        scope
            .SetupGet(S => S.ServiceProvider)
            .Returns(serviceProvider.Object);

        _scopeFactory
            .Setup(F => F.CreateScope())
            .Returns(scope.Object);
    }

    private static async Task RunOnceAsync(
        ImportQueueBackgroundService service,
        Mock<IImportJobQueue> queue,
        int importJobId)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var dequeueCount = 0;

        queue
            .Setup(Q => Q.DequeueAsync(It.IsAny<CancellationToken>()))
            .Returns((CancellationToken token) =>
            {
                dequeueCount++;

                if (dequeueCount == 1)
                {
                    return ValueTask.FromResult(importJobId);
                }

                return new ValueTask<int>(
                    Task.FromCanceled<int>(token));
            });

        await service.StartAsync(cancellationTokenSource.Token);

        // Give the background task an opportunity to process the item.
        await Task.Delay(50);

        cancellationTokenSource.Cancel();

        await service.StopAsync(CancellationToken.None);
    }
    private void SetupQueueForSingleJob(int importJobId)
    {
        var callCount = 0;

        _queue
            .Setup(Q => Q.DequeueAsync(It.IsAny<CancellationToken>()))
            .Returns((CancellationToken cancellationToken) =>
            {
                callCount++;

                if (callCount == 1)
                {
                    return ValueTask.FromResult(importJobId);
                }

                return new ValueTask<int>(
                    WaitForCancellationAsync(cancellationToken));
            });
    }

    private Mock<IImportJobRepository> CreateJobRepository()
    {
        return new Mock<IImportJobRepository>();
    }

    private Mock<IImportJobItemRepository> CreateItemRepository()
    {
        return new Mock<IImportJobItemRepository>();
    }

    private Mock<IMediator> CreateMediator()
    {
        return new Mock<IMediator>();
    }

    private Mock<ICosmicLatteAPIService> CreateCosmicLatte()
    {
        return new Mock<ICosmicLatteAPIService>();
    }

    private static async Task<int> WaitForCancellationAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(Timeout.Infinite, cancellationToken);
        return 0;
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueueIsCancelled_StopsProcessingAsync()
    {
        using var cancellationTokenSource = new CancellationTokenSource();

        _queue.Setup(Q => Q.DequeueAsync(It.IsAny<CancellationToken>()))
            .Returns((CancellationToken token) => new ValueTask<int>(
                    WaitForCancellationAsync(token)));

        var service = CreateService();
        await service.StartAsync(cancellationTokenSource.Token);
        cancellationTokenSource.Cancel();
        await service.StopAsync(CancellationToken.None);

        _scopeFactory.Verify(F => F.CreateScope(), Times.Never);
        _queue.Verify(Q => Q.DequeueAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenProcessingAndMarkingFailedBothThrow_DoesNotPropagateExceptionAsync()
    {
        var jobRepository = new Mock<IImportJobRepository>();

        jobRepository
            .Setup(R => R.GetByIdAsync(123))
            .ThrowsAsync(new InvalidOperationException("Processing failed"));

        jobRepository
            .Setup(R => R.SetResultAsync(123, ImportJobStatus.Failed, 0, "Processing failed", It.IsAny<DateTime>()))
            .ThrowsAsync(new InvalidOperationException("Could not update job"));

        SetupScope(jobRepository);
        SetupQueueForSingleJob(123);
        var service = CreateService();
        using var cancellationTokenSource = new CancellationTokenSource();

        await service.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(100);

        cancellationTokenSource.Cancel();
        await service.StopAsync(CancellationToken.None);

        jobRepository.Verify(
            R => R.SetResultAsync(123, ImportJobStatus.Failed, 0, "Processing failed", It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenProcessingThrows_MarksJobAsFailedAsync()
    {
        var jobRepository = new Mock<IImportJobRepository>();

        jobRepository
            .Setup(R => R.GetByIdAsync(123))
            .ThrowsAsync(new InvalidOperationException("Something went wrong"));

        SetupScope(jobRepository);
        SetupQueueForSingleJob(123);

        var service = CreateService();
        using var cancellationTokenSource = new CancellationTokenSource();

        await service.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(100);
        cancellationTokenSource.Cancel();
        await service.StopAsync(CancellationToken.None);

        jobRepository.Verify(
            R => R.SetResultAsync(123, ImportJobStatus.Failed, 0, "Something went wrong", It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSomeItemsCompletedAndSomeFailed_SetsPartiallyCompletedAsync()
    {
        var jobRepository = new Mock<IImportJobRepository>();
        var itemRepository = new Mock<IImportJobItemRepository>();
        var orchestrator = new Mock<IPollOrchestratorServiceV2>();

        var job = new ImportJob
        {
            Id = 123,
            EvaluationId = 456,
            Status = ImportJobStatus.Ready
        };

        var item = new ImportJobItem
        {
            Id = 1,
            ImportJobId = 123,
            Status = ImportJobStatus.Queued,
            PollPayload = JsonSerializer.Serialize(new PollDTO())
        };

        jobRepository
            .Setup(R => R.GetByIdAsync(123))
            .ReturnsAsync(job);

        itemRepository
            .Setup(R => R.GetByJobIdAndStatusAsync(123, ImportJobStatus.Queued))
            .ReturnsAsync(new List<ImportJobItem> { item });

        var response = new CreateCommandResponse<Poll>(null, "Success", true);

        orchestrator
            .Setup(O => O.SetupImportStructureAsync(It.IsAny<List<PollDTO>>(), 456))
            .ReturnsAsync(response);

        ImportStudentResult orchestratorResponse = new ImportStudentResult(true, "");

        orchestrator
            .Setup(O => O.ProcessStudentAsync(It.IsAny<PollDTO>(), 456))
            .ReturnsAsync(orchestratorResponse);

        itemRepository
            .Setup(R => R.GetImportPhaseCountsAsync(123))
            .ReturnsAsync((0, 3, 2));

        SetupScope(jobRepository, itemRepository, orchestrator);
        SetupQueueForSingleJob(123);

        var service = CreateService();
        using var cancellationTokenSource = new CancellationTokenSource();

        await service.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(100);
        cancellationTokenSource.Cancel();
        await service.StopAsync(CancellationToken.None);

        jobRepository.Verify(
            R => R.SetResultAsync(123, ImportJobStatus.PartiallyCompleted, 3, null, It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOrchestratorFailed_SetsPartiallyCompletedAsync()
    {
        var jobRepository = new Mock<IImportJobRepository>();
        var itemRepository = new Mock<IImportJobItemRepository>();
        var orchestrator = new Mock<IPollOrchestratorServiceV2>();

        var job = new ImportJob
        {
            Id = 123,
            EvaluationId = 456,
            Status = ImportJobStatus.Ready
        };

        var item = new ImportJobItem
        {
            Id = 1,
            ImportJobId = 123,
            Status = ImportJobStatus.Queued,
            PollPayload = JsonSerializer.Serialize(new PollDTO())
        };

        jobRepository
            .Setup(R => R.GetByIdAsync(123))
            .ReturnsAsync(job);

        itemRepository
            .Setup(R => R.GetByJobIdAndStatusAsync(123, ImportJobStatus.Queued))
            .ReturnsAsync(new List<ImportJobItem> { item });

        var response = new CreateCommandResponse<Poll>(null, "Failed", false);

        orchestrator
            .Setup(O => O.SetupImportStructureAsync(It.IsAny<List<PollDTO>>(), 456))
            .ReturnsAsync(response);

        ImportStudentResult orchestratorResponse = new ImportStudentResult(true, "");

        orchestrator
            .Setup(O => O.ProcessStudentAsync(It.IsAny<PollDTO>(), 456))
            .ReturnsAsync(orchestratorResponse);

        itemRepository
            .Setup(R => R.GetImportPhaseCountsAsync(123))
            .ReturnsAsync((0, 3, 2));

        SetupScope(jobRepository, itemRepository, orchestrator);
        SetupQueueForSingleJob(123);

        var service = CreateService();
        using var cancellationTokenSource = new CancellationTokenSource();

        await service.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(100);
        cancellationTokenSource.Cancel();
        await service.StopAsync(CancellationToken.None);

        jobRepository.Verify(
            R => R.SetStatusAsync(123, ImportJobStatus.Importing, It.IsAny<DateTime>()),
            Times.Once);
        jobRepository.Verify(
            R => R.SetResultAsync(123, ImportJobStatus.Failed, 0, "Failed", It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStudentImportFails_MarksItemFailedAsync()
    {
        var jobRepository = new Mock<IImportJobRepository>();
        var itemRepository = new Mock<IImportJobItemRepository>();
        var orchestrator = new Mock<IPollOrchestratorServiceV2>();

        var job = new ImportJob
        {
            Id = 123,
            EvaluationId = 456,
            Status = ImportJobStatus.Ready
        };

        var item = new ImportJobItem
        {
            Id = 1,
            ImportJobId = 123,
            Status = ImportJobStatus.Queued,
            PollPayload = JsonSerializer.Serialize(new PollDTO())
        };

        jobRepository
            .Setup(R => R.GetByIdAsync(123))
            .ReturnsAsync(job);

        itemRepository
            .Setup(R => R.GetByJobIdAndStatusAsync(123,
                ImportJobStatus.Queued))
            .ReturnsAsync(new List<ImportJobItem> { item });

        var responseOrchestrator = new CreateCommandResponse<Poll>(null, "Success", true);
        orchestrator
            .Setup(O => O.SetupImportStructureAsync(
                It.IsAny<List<PollDTO>>(),
                456))
            .ReturnsAsync(responseOrchestrator);

        var imported = new ImportStudentResult(false, "Student import failed");

        orchestrator
            .Setup(O => O.ProcessStudentAsync(It.IsAny<PollDTO>(), 456))
            .ReturnsAsync(imported);

        itemRepository
            .Setup(R => R.GetImportPhaseCountsAsync(123))
            .ReturnsAsync((0, 0, 1));

        SetupScope(jobRepository, itemRepository, orchestrator);

        SetupQueueForSingleJob(123);
        var service = CreateService();
        using var cancellationTokenSource = new CancellationTokenSource();

        await service.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(100);
        cancellationTokenSource.Cancel();
        await service.StopAsync(CancellationToken.None);

        itemRepository.Verify(
            R => R.SetStatusAsync(1, ImportJobStatus.Failed, "Student import failed", It.IsAny<DateTime>()),
            Times.Once);

        jobRepository.Verify(
            R => R.SetResultAsync(123, ImportJobStatus.Failed, 0, null, It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueuedItemsProcessSuccessfully_MarksItemsCompletedAndUpdatesAggregateAsync()
    {
        var jobRepository = new Mock<IImportJobRepository>();
        var itemRepository = new Mock<IImportJobItemRepository>();
        var orchestrator = new Mock<IPollOrchestratorServiceV2>();
        var job = new ImportJob
        {
            Id = 123,
            EvaluationId = 456,
            Status = ImportJobStatus.Ready
        };

        var item1 = new ImportJobItem
        {
            Id = 1,
            ImportJobId = 123,
            Status = ImportJobStatus.Queued,
            PollPayload = JsonSerializer.Serialize(new PollDTO())
        };

        var item2 = new ImportJobItem
        {
            Id = 2,
            ImportJobId = 123,
            Status = ImportJobStatus.Queued,
            PollPayload = JsonSerializer.Serialize(new PollDTO())
        };

        jobRepository
            .Setup(R => R.GetByIdAsync(123))
            .ReturnsAsync(job);

        itemRepository
            .Setup(R => R.GetByJobIdAndStatusAsync(123, ImportJobStatus.Queued))
            .ReturnsAsync(new List<ImportJobItem> { item1, item2 });

        var response = new CreateCommandResponse<Poll>(null, "Success", true);

        orchestrator
            .Setup(O => O.SetupImportStructureAsync(
                It.IsAny<List<PollDTO>>(),
                456))
            .ReturnsAsync(response);

        ImportStudentResult orchestratorResponse = new ImportStudentResult(true, "");
        orchestrator
            .Setup(O => O.ProcessStudentAsync(It.IsAny<PollDTO>(), 456))
            .ReturnsAsync(orchestratorResponse);

        itemRepository
            .Setup(R => R.GetImportPhaseCountsAsync(123))
            .ReturnsAsync((0, 2, 0));

        SetupScope(jobRepository, itemRepository, orchestrator);

        SetupQueueForSingleJob(123);

        var service = CreateService();

        using var cancellationTokenSource = new CancellationTokenSource();

        await service.StartAsync(cancellationTokenSource.Token);

        await Task.Delay(100);

        cancellationTokenSource.Cancel();

        await service.StopAsync(CancellationToken.None);

        jobRepository.Verify(
            R => R.SetStatusAsync(123, ImportJobStatus.Importing, It.IsAny<DateTime>()),
            Times.Once);

        itemRepository.Verify(
            R => R.SetStatusAsync(1, ImportJobStatus.Running, null, It.IsAny<DateTime>()),
            Times.Once);

        itemRepository.Verify(
            R => R.SetStatusAsync(1, ImportJobStatus.Completed, "", It.IsAny<DateTime>()),
            Times.Once);

        itemRepository.Verify(
            R => R.SetStatusAsync(2, ImportJobStatus.Running, null, It.IsAny<DateTime>()),
            Times.Once);

        itemRepository.Verify(
            R => R.SetStatusAsync(2, ImportJobStatus.Completed, "", It.IsAny<DateTime>()),
            Times.Once);

        jobRepository.Verify(
            R => R.SetResultAsync(123, ImportJobStatus.Completed, 2, null, It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenImportingJobHasNoQueuedItems_UpdatesAggregateStatusAsync()
    {
        var jobRepository = new Mock<IImportJobRepository>();
        var itemRepository = new Mock<IImportJobItemRepository>();
        var orchestrator = new Mock<IPollOrchestratorServiceV2>();

        var job = new ImportJob
        {
            Id = 123,
            EvaluationId = 456,
            Status = ImportJobStatus.Ready
        };

        jobRepository
            .Setup(R => R.GetByIdAsync(123))
            .ReturnsAsync(job);

        itemRepository
            .Setup(R => R.GetByJobIdAndStatusAsync(
                123,
                ImportJobStatus.Queued))
            .ReturnsAsync(new List<ImportJobItem>());

        itemRepository
            .Setup(R => R.GetImportPhaseCountsAsync(123))
            .ReturnsAsync((0, 5, 0));

        SetupScope(jobRepository, itemRepository, orchestrator);
        SetupQueueForSingleJob(123);

        var service = CreateService();
        using var cancellationTokenSource = new CancellationTokenSource();
        await service.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(100);
        cancellationTokenSource.Cancel();
        await service.StopAsync(CancellationToken.None);

        jobRepository.Verify(
            R => R.SetResultAsync(123, ImportJobStatus.Completed, 5, null, It.IsAny<DateTime>()),
            Times.Once);

        jobRepository.Verify(
            R => R.SetStatusAsync(It.IsAny<int>(), It.IsAny<ImportJobStatus>(), It.IsAny<DateTime>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenJobIsExtracting_ExtractsRespondentsAndMarksJobReadyAsync()
    {
        var jobRepository = CreateJobRepository();
        var itemRepository = CreateItemRepository();
        var mediator = CreateMediator();
        var cosmicLatte = CreateCosmicLatte();

        var job = new ImportJob
        {
            Id = 123,
            Status = ImportJobStatus.Extracting,
            ConfigurationId = 10,
            EvaluationSetName = "EvaluationSet",
            StartDate = "2026-01-01",
            EndDate = "2026-01-31",
            PollId = "Poll1"
        };

        jobRepository
            .Setup(R => R.GetByIdAsync(123))
            .ReturnsAsync(job);

        mediator
            .Setup(M => M.Send(
                It.Is<GetConfigurationQuery>(
                    Q => Q.ConfigurationId == 10),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Configurations
            {
                UserId = "12",
                ConfigurationName = "new",
                EncryptedKey = "encrypted-key",
                BaseURL = "https://example.com"
            });

        cosmicLatte
            .Setup(C => C.ExtractRespondentsAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Func<PollDTO, bool, Task>>()))
            .Returns(
                async (
                    string evaluationSet,
                    string startDate,
                    string endDate,
                    string pollId,
                    string encryptedKey,
                    string baseUrl,
                    Func<PollDTO, bool, Task> callback) =>
                {
                    await callback(
                        new PollDTO(),
                        false);

                    await callback(
                        new PollDTO(),
                        true);
                });

        SetupScope(
            jobRepository,
            itemRepository,
            null,
            mediator,
            cosmicLatte);

        SetupQueueForSingleJob(123);
        var service = CreateService();
        using var cancellationTokenSource = new CancellationTokenSource();
        await service.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(100);

        cancellationTokenSource.Cancel();

        await service.StopAsync(CancellationToken.None);

        mediator.Verify(
            M => M.Send(It.Is<GetConfigurationQuery>(Q => Q.ConfigurationId == 10), It.IsAny<CancellationToken>()),
            Times.Once);

        cosmicLatte.Verify(
            C => C.ExtractRespondentsAsync(
                "EvaluationSet", "2026-01-01", "2026-01-31", "Poll1", "encrypted-key", "https://example.com", It.IsAny<Func<PollDTO, bool, Task>>()),
            Times.Once);

        itemRepository.Verify(
            R => R.AddAsync(It.Is<ImportJobItem>(I => I.ImportJobId == 123 && I.Status == ImportJobStatus.Extracted)),
            Times.Exactly(2));

        jobRepository.Verify(
            R => R.SetExtractedCountAsync(123, 1, It.IsAny<DateTime>()),
            Times.Once);

        jobRepository.Verify(
            R => R.SetExtractedCountAsync(123, 2, It.IsAny<DateTime>()),
            Times.Once);

        jobRepository.Verify(
            R => R.SetReadyAsync(123, 2, It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenJobDoesNotExist_LogsWarningAndSkipsJobAsync()
    {
        var repository = new Mock<IImportJobRepository>();

        repository
            .Setup(R => R.GetByIdAsync(123))
            .ReturnsAsync((ImportJob?)null);

        SetupScope(repository);
        SetupQueueForSingleJob(123);
        var service = CreateService();
        using var cancellationTokenSource = new CancellationTokenSource();
        await service.StartAsync(cancellationTokenSource.Token);

        await Task.Delay(50);

        cancellationTokenSource.Cancel();

        await service.StopAsync(CancellationToken.None);

        repository.Verify(R => R.GetByIdAsync(123), Times.Once);

        _scopeFactory.Verify(F => F.CreateScope(), Times.Once);
    }

}
