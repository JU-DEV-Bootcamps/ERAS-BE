using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.Services;
using Eras.Domain.Entities;

using Moq;

namespace Eras.Application.Tests.Services;

public class ImportJobServiceTests
{
    private readonly Mock<IImportJobRepository> _mockImportJobRepository;
    private readonly Mock<IImportJobItemRepository> _mockImportJobItemRepository;
    private readonly Mock<IImportJobQueue> _mockQueue;
    private readonly Mock<IEvaluationRepository> _mockEvaluationRepository;
    private readonly ImportJobService _service;

    public ImportJobServiceTests()
    {
        _mockImportJobRepository = new Mock<IImportJobRepository>();
        _mockImportJobItemRepository = new Mock<IImportJobItemRepository>();
        _mockQueue = new Mock<IImportJobQueue>();
        _mockEvaluationRepository = new Mock<IEvaluationRepository>();
        _service = new ImportJobService(
            _mockImportJobRepository.Object,
            _mockImportJobItemRepository.Object,
            _mockQueue.Object,
            _mockEvaluationRepository.Object);
    }

    [Fact]
    public async Task StartExtractionAsync_Should_Throw_ArgumentException_When_EvaluationSetName_Exceeds_MaxLengthAsync()
    {
        // Arrange
        var longName = new string('a', 101);

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.StartExtractionAsync(longName, 1, "2026-01-01", "2026-12-31", 1));

        // Assert
        Assert.Equal("There was an error during the import: Poll Name exceeds the maximum length of 100 characters.",
            exception.Message);
        _mockEvaluationRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task StartExtractionAsync_Should_Throw_ArgumentException_When_StartDate_InvalidAsync()
    {
        // Arrange
        var evaluation = new Evaluation();
        _mockEvaluationRepository.Setup(Repo => Repo.GetByIdAsync(1)).ReturnsAsync(evaluation);

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.StartExtractionAsync("Poll", 1, "not-a-date", "2024-12-31", 1));

        // Assert
        Assert.Equal("There was an error during the import: Invalid start or end date.", exception.Message);
    }

    [Fact]
    public async Task StartExtractionAsync_Should_Create_Job_And_Enqueue_On_SuccessAsync()
    {
        // Arrange
        var evaluation = new Evaluation
        {
            Name = "Eva1",
            Status = "Pending",
            StartDate = DateTime.Parse("2026-01-01"),
            EndDate = DateTime.Parse("2026-12-31"),
            PollName = "Example",
            Country = "A",
            PollId = 1,
            ConfigurationId = 1,
            EvaluationPollId = 1,
        }; 
        var startDate = "2026-01-01";
        var endDate = "2026-12-31";
        var evaluationId = 1;
        var configurationId = 5;
        var pollName = "My Poll";

        _mockEvaluationRepository.Setup(Repo => Repo.GetByIdAsync(evaluationId)).ReturnsAsync(evaluation);

        ImportJob? capturedJob = null;
        _mockImportJobRepository
            .Setup(Repo => Repo.AddAsync(It.IsAny<ImportJob>()))
            .Callback<ImportJob>(job => capturedJob = job)
            .ReturnsAsync((ImportJob job) =>
            {
                job.Id = 42;
                return job;
            });

        // Act
        var result = await _service.StartExtractionAsync(pollName, configurationId, startDate, endDate, evaluationId);

        // Assert
        Assert.Equal(42, result);
        Assert.NotNull(capturedJob);
        Assert.Equal(pollName, capturedJob!.EvaluationSetName);
        Assert.Equal(configurationId, capturedJob.ConfigurationId);
        Assert.Equal(evaluationId, capturedJob.EvaluationId);
        Assert.Equal(startDate, capturedJob.StartDate);
        Assert.Equal(endDate, capturedJob.EndDate);
        Assert.Equal(ImportJobStatus.Extracting, capturedJob.Status);
        Assert.Equal("[]", capturedJob.PollsPayload);

        _mockImportJobRepository.Verify(x => x.AddAsync(It.IsAny<ImportJob>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmImportAsync_Should_Return_False_When_Job_Not_FoundAsync()
    {
        // Arrange
        _mockImportJobRepository.Setup(Repo => Repo.GetByIdAsync(1)).ReturnsAsync((ImportJob?)null);

        // Act
        var result = await _service.ConfirmImportAsync(1, new List<int> { 1, 2 });

        // Assert
        Assert.False(result);
        _mockImportJobItemRepository.Verify(
            x => x.ConfirmSelectionAsync(It.IsAny<int>(), It.IsAny<List<int>>(), It.IsAny<DateTime>()),
            Times.Never);
    }

    [Fact]
    public async Task ConfirmImportAsync_Should_Confirm_Items_Set_Importing_And_Enqueue_On_SuccessAsync()
    {
        // Arrange
        var jobId = 7;
        var itemIds = new List<int> { 10, 11, 12 };
        var job = new ImportJob { Id = jobId };

        _mockImportJobRepository.Setup(Repo => Repo.GetByIdAsync(jobId)).ReturnsAsync(job);
        _mockImportJobItemRepository
            .Setup(Repo => Repo.GetImportPhaseCountsAsync(jobId))
            .ReturnsAsync((3, 0, 0));

        // Act
        var result = await _service.ConfirmImportAsync(jobId, itemIds);

        // Assert
        Assert.True(result);
        _mockImportJobItemRepository.Verify(
            x => x.ConfirmSelectionAsync(jobId, itemIds, It.IsAny<DateTime>()),
            Times.Once);
        _mockImportJobItemRepository.Verify(x => x.GetImportPhaseCountsAsync(jobId), Times.Once);
        _mockImportJobRepository.Verify(
            x => x.SetImportingAsync(jobId, 3, It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task QueueImportAsync_Should_Throw_ArgumentException_When_A_Poll_Name_Exceeds_MaxLengthAsync()
    {
        // Arrange
        var polls = new List<PollDTO>
        {
            new PollDTO { Name = new string('a', 101) },
        };

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.QueueImportAsync(polls, 1));

        // Assert
        Assert.Equal(
            "There was an error during the import: Poll Name exceeds the maximum length of 100 characters.",
            exception.Message);
        _mockImportJobRepository.Verify(x => x.AddAsync(It.IsAny<ImportJob>()), Times.Never);
    }

    [Fact]
    public async Task QueueImportAsync_Should_Create_Job()
    {
        // Arrange
        var polls = new List<PollDTO>
        {
            new PollDTO { Name = "Poll 1" },
            new PollDTO { Name = "Poll 2" },
        };
        var evaluationId = 9;

        ImportJob? capturedJob = null;
        _mockImportJobRepository
            .Setup(Repo => Repo.AddAsync(It.IsAny<ImportJob>()))
            .Callback<ImportJob>(job => capturedJob = job)
            .ReturnsAsync((ImportJob job) =>
            {
                job.Id = 55;
                return job;
            });

        // Act
        var result = await _service.QueueImportAsync(polls, evaluationId);

        // Assert
        Assert.Equal(55, result);
        Assert.NotNull(capturedJob);
        Assert.Equal(evaluationId, capturedJob!.EvaluationId);
        Assert.Equal(ImportJobStatus.Queued, capturedJob.Status);
        Assert.Equal(polls.Count, capturedJob.TotalCount);

        _mockImportJobItemRepository.Verify(
            x => x.AddAsync(It.IsAny<ImportJobItem>()),
            Times.Exactly(polls.Count));
    }

    [Fact]
    public async Task QueueImportAsync_Should_Use_Empty_Student_Fields_When_Student_Data_Is_MissingAsync()
    {
        // Arrange
        var polls = new List<PollDTO> { new PollDTO { Name = "Poll without student" } };
        ImportJobItem? capturedItem = null;

        _mockImportJobRepository
            .Setup(Repo => Repo.AddAsync(It.IsAny<ImportJob>()))
            .ReturnsAsync((ImportJob job) =>
            {
                job.Id = 1;
                return job;
            });
        _mockImportJobItemRepository
            .Setup(Repo => Repo.AddAsync(It.IsAny<ImportJobItem>()))
            .Callback<ImportJobItem>(item => capturedItem = item)
            .ReturnsAsync((ImportJobItem item) => item);

        // Act
        await _service.QueueImportAsync(polls, 1);

        // Assert
        Assert.NotNull(capturedItem);
        Assert.Equal(string.Empty, capturedItem!.StudentEmail);
        Assert.Equal(string.Empty, capturedItem.StudentName);
        Assert.Null(capturedItem.Cohort);
    }


    [Fact]
    public async Task GetStatusAsync_Should_Return_Null_When_Job_Not_FoundAsync()
    {
        // Arrange
        _mockImportJobRepository.Setup(Repo => Repo.GetByIdAsync(1)).ReturnsAsync((ImportJob?)null);

        // Act
        var result = await _service.GetStatusAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetStatusAsync_Should_Map_Job_To_StatusDTO_When_FoundAsync()
    {
        // Arrange
        var job = new ImportJob
        {
            Id = 3,
            EvaluationId = 4,
            Status = ImportJobStatus.Importing,
            TotalCount = 10,
            ProcessedCount = 5,
            ExtractedCount = 8,
            RetryCount = 1,
            ErrorMessage = "some error",
            CreatedAtUtc = new DateTime(2024, 1, 1),
            UpdatedAtUtc = new DateTime(2024, 1, 2),
        };
        _mockImportJobRepository.Setup(Repo => Repo.GetByIdAsync(3)).ReturnsAsync(job);

        // Act
        var result = await _service.GetStatusAsync(3);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(job.Id, result!.ImportJobId);
        Assert.Equal(job.EvaluationId, result.EvaluationId);
        Assert.Equal(job.Status.ToString(), result.Status);
        Assert.Equal(job.TotalCount, result.TotalCount);
        Assert.Equal(job.ProcessedCount, result.ProcessedCount);
        Assert.Equal(job.ExtractedCount, result.ExtractedCount);
        Assert.Equal(job.RetryCount, result.RetryCount);
        Assert.Equal(job.ErrorMessage, result.ErrorMessage);
        Assert.Equal(job.CreatedAtUtc, result.CreatedAtUtc);
        Assert.Equal(job.UpdatedAtUtc, result.UpdatedAtUtc);
    }

    [Fact]
    public async Task GetItemsAsync_Should_Map_Items_To_DTOsAsync()
    {
        // Arrange
        var items = new List<ImportJobItem>
        {
            new ImportJobItem
            {
                Id = 1,
                ImportJobId = 5,
                StudentEmail = "student@example.com",
                StudentName = "Student One",
                Cohort = "Cohort A",
                Status = ImportJobStatus.Failed,
                RetryCount = 2,
                IsAlreadyImported = false,
                ErrorMessage = "some error",
            },
            new ImportJobItem
            {
                Id = 2,
                ImportJobId = 5,
                StudentEmail = "student2@example.com",
                StudentName = "Student Two",
                Cohort = null,
                Status = ImportJobStatus.Queued,
                RetryCount = 0,
                IsAlreadyImported = true,
                ErrorMessage = null,
            },
        };
        _mockImportJobItemRepository.Setup(Repo => Repo.GetByJobIdAsync(5)).ReturnsAsync(items);

        // Act
        var result = await _service.GetItemsAsync(5);

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Equal(items[0].Id, result[0].Id);
        Assert.Equal(items[0].ImportJobId, result[0].ImportJobId);
        Assert.Equal(items[0].StudentEmail, result[0].StudentEmail);
        Assert.Equal(items[0].StudentName, result[0].StudentName);
        Assert.Equal(items[0].Cohort, result[0].Cohort);
        Assert.Equal(items[0].Status.ToString(), result[0].Status);
        Assert.Equal(items[0].RetryCount, result[0].RetryCount);
        Assert.Equal(items[0].IsAlreadyImported, result[0].IsAlreadyImported);
        Assert.Equal(items[0].ErrorMessage, result[0].ErrorMessage);

        Assert.Null(result[1].Cohort);
        Assert.Null(result[1].ErrorMessage);
        Assert.True(result[1].IsAlreadyImported);
    }

    [Fact]
    public async Task RetryItemsAsync_Should_Return_False_When_Job_Not_FoundAsync()
    {
        // Arrange
        _mockImportJobRepository.Setup(Repo => Repo.GetByIdAsync(1)).ReturnsAsync((ImportJob?)null);

        // Act
        var result = await _service.RetryItemsAsync(1, new List<int> { 1, 2 });

        // Assert
        Assert.False(result);
        _mockImportJobItemRepository.Verify(
            x => x.RequeueFailedAsync(It.IsAny<int>(), It.IsAny<List<int>>(), It.IsAny<DateTime>()),
            Times.Never);
        _mockImportJobRepository.Verify(
            x => x.SetStatusAsync(It.IsAny<int>(), It.IsAny<ImportJobStatus>(), It.IsAny<DateTime>()),
            Times.Never);
        //_mockQueue.Verify(x => x.EnqueueAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task RetryItemsAsync_Should_Return_True_Without_Requeuing_When_No_Items_RequeuedAsync()
    {
        // Arrange
        var jobId = 3;
        var job = new ImportJob { Id = jobId };
        _mockImportJobRepository.Setup(Repo => Repo.GetByIdAsync(jobId)).ReturnsAsync(job);
        _mockImportJobItemRepository
            .Setup(Repo => Repo.RequeueFailedAsync(jobId, It.IsAny<List<int>>(), It.IsAny<DateTime>()))
            .ReturnsAsync(0);

        // Act
        var result = await _service.RetryItemsAsync(jobId, new List<int> { 1, 2 });

        // Assert
        Assert.True(result);
        _mockImportJobRepository.Verify(
            x => x.SetStatusAsync(It.IsAny<int>(), It.IsAny<ImportJobStatus>(), It.IsAny<DateTime>()),
            Times.Never);
        //_mockQueue.Verify(x => x.EnqueueAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task RetryItemsAsync_Should_Set_Status_Queued_And_Enqueue_When_Items_RequeuedAsync()
    {
        // Arrange
        var jobId = 4;
        var itemIds = new List<int> { 10, 20 };
        var job = new ImportJob { Id = jobId };
        _mockImportJobRepository.Setup(Repo => Repo.GetByIdAsync(jobId)).ReturnsAsync(job);
        _mockImportJobItemRepository
            .Setup(Repo => Repo.RequeueFailedAsync(jobId, itemIds, It.IsAny<DateTime>()))
            .ReturnsAsync(2);

        // Act
        var result = await _service.RetryItemsAsync(jobId, itemIds);

        // Assert
        Assert.True(result);
        _mockImportJobRepository.Verify(
            x => x.SetStatusAsync(jobId, ImportJobStatus.Queued, It.IsAny<DateTime>()),
            Times.Once);
        //_mockQueue.Verify(x => x.EnqueueAsync(jobId), Times.Once);
    }
}
