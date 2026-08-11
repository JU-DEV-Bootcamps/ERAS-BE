using Eras.Application.Contracts.Persistence;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class EvaluationRepositoryTest
{
    private Mock<DbSet<EvaluationEntity>> _mockSet;
    protected Mock<AppDbContext> _mockContext;
    private IEvaluationRepository? _repository;

    public EvaluationRepositoryTest()
    {
        _mockSet = new Mock<DbSet<EvaluationEntity>>();
        _mockContext = new Mock<AppDbContext>(
            new DbContextOptions<AppDbContext>());
    }

    private void SetupEvaluations(IEnumerable<EvaluationEntity> evaluations)
    {
        var data = evaluations
            .AsQueryable()
            .BuildMockDbSet();

        _mockContext
            .Setup(c => c.Evaluations)
            .Returns(data.Object);
    }

    private void CreateRepository()
    {
        _repository = new EvaluationRepository(_mockContext.Object);
    }

    [Fact]
    public async Task GetByNameAsync_WhenEvaluationExists_ReturnsEvaluationAsync()
    {
        // Arrange
        SetupEvaluations(
        [
            new EvaluationEntity
            {
                Id = 1,
                Name = "Evaluation1"
            },
            new EvaluationEntity
            {
                Id = 2,
                Name = "Evaluation2"
            }
        ]);
        CreateRepository();

        // Act
        var result = await _repository!.GetByNameAsync("Evaluation1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Evaluation1", result.Name);
    }

    [Fact]
    public async Task GetByNameAsync_WhenEvaluationDoesNotExist_ReturnsNullAsync()
    {
        // Arrange
        SetupEvaluations([
            new EvaluationEntity
            {
                Id = 1,
                Name = "Evaluation1"
            }
        ]);

        CreateRepository();

        // Act
        var result = await _repository!.GetByNameAsync("DoesNotExist");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdForUpdateAsync_WhenEvaluationExists_ReturnsEvaluationAsync()
    {
        // Arrange
        SetupEvaluations(
        [
            new EvaluationEntity
            {
                Id = 10,
                Name = "Evaluation 10"
            }
        ]);

        CreateRepository();

        // Act
        var result = await _repository!.GetByIdForUpdateAsync(10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Evaluation 10", result.Name);
    }

    [Fact]
    public async Task GetByIdForUpdateAsync_WhenEvaluationDoesNotExist_ReturnsNullAsync()
    {
        // Arrange
        SetupEvaluations(
        [
            new EvaluationEntity
            {
                Id = 10,
                Name = "Evaluation 10"
            }
        ]);

        CreateRepository();

        // Act
        var result = await _repository!.GetByIdForUpdateAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsEvaluationsOrderedByName()
    {
        // Arrange
        var poll1 = new PollEntity
        {
            Uuid = "poll-1"
        };
        var poll2 = new PollEntity
        {
            Uuid = "poll-2"
        };

        var evaluations = new List<EvaluationEntity>
        {
            new()
            {
                Id = 1,
                Name = "Z Evaluation",
                EvaluationPolls =
                [
                    new EvaluationPollJoin
                    {
                        EvaluationId = 1,
                        Poll = poll1
                    }
                ]
            },
            new()
            {
                Id = 2,
                Name = "A Evaluation",
                EvaluationPolls =
                [
                    new EvaluationPollJoin
                    {
                        EvaluationId = 2,
                        Poll = poll2
                    }
                ]
            }
        };
        SetupEvaluations(evaluations);
        CreateRepository();

        // Act
        var result = (await _repository!.GetPagedAsync(1, 10)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("A Evaluation", result[0].Name);
        Assert.Equal("Z Evaluation", result[1].Name);
        Assert.Single(result[0].Polls);
        Assert.Equal("poll-2", result[0].Polls.First().Uuid);
        Assert.Single(result[1].Polls);
        Assert.Equal("poll-1", result[1].Polls.First().Uuid);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectPageAsync()
    {
        // Arrange
        SetupEvaluations(
        [
            new EvaluationEntity
            {
                Id = 1,
                Name = "A"
            },
            new EvaluationEntity
            {
                Id = 2,
                Name = "B"
            },
            new EvaluationEntity
            {
                Id = 3,
                Name = "C"
            }
        ]);
        CreateRepository();

        // Act
        var result = (await _repository!.GetPagedAsync(2, 1)).ToList();

        // Assert
        var evaluation = Assert.Single(result);
        Assert.Equal(2, evaluation.Id);
        Assert.Equal("B", evaluation.Name);
    }

    [Fact]
    public async Task GetPagedAsync_WhenPageIsBeyondData_ReturnsEmptyAsync()
    {
        // Arrange
        SetupEvaluations(
        [
            new EvaluationEntity
            {
                Id = 1,
                Name = "A"
            }
        ]);

        CreateRepository();

        // Act
        var result = (await _repository!.GetPagedAsync(10, 10)).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByNameForUpdateAsync_ReturnsEvaluationWithSameNameButDifferentIdAsync()
    {
        // Arrange
        SetupEvaluations(
        [
            new EvaluationEntity
        {
            Id = 1,
            Name = "Existing"
        },
        new EvaluationEntity
        {
            Id = 2,
            Name = "Another"
        }
        ]);

        CreateRepository();

        // Act
        var result = await _repository!.GetByNameForUpdateAsync(2, "Existing");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Existing", result.Name);
    }

    [Fact]
    public async Task GetByNameForUpdateAsync_DoesNotReturnSameEvaluationAsync()
    {
        // Arrange
        SetupEvaluations(
        [
            new EvaluationEntity
            {
                Id = 1,
                Name = "Existing"
            }
        ]);

        CreateRepository();

        // Act
        var result = await _repository!.GetByNameForUpdateAsync(1, "Existing");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameForUpdateAsync_WhenNameDoesNotExist_ReturnsNullAsync()
    {
        // Arrange
        SetupEvaluations(
        [
            new EvaluationEntity
        {
            Id = 1,
            Name = "Existing"
        }
        ]);

        CreateRepository();

        // Act
        var result = await _repository!.GetByNameForUpdateAsync(1, "DoesNotExist");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetStatusById_WhenEvaluationExists_ReturnsEvaluationAsync()
    {
        // Arrange
        SetupEvaluations(
        [
            new EvaluationEntity
            {
                Id = 10,
                Name = "Evaluation",
                Status = "Completed",
                PollName = "Poll",
                Country = "Bolivia",
                ConfigurationId = 5
            }
        ]);
        CreateRepository();

        // Act
        var result = await _repository!.GetStatusById(10);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(10, result.Id);
        Assert.Equal("Evaluation", result.Name);
        Assert.Equal("Completed", result.Status);
        Assert.Equal("Poll", result.PollName);
        Assert.Equal("Bolivia", result.Country);
        Assert.Equal(5, result.ConfigurationId);
    }

    [Fact]
    public async Task GetStatusById_WhenEvaluationDoesNotExist_ReturnsNullAsync()
    {
        // Arrange
        SetupEvaluations(
        [
            new EvaluationEntity
            {
                Id = 10,
                Name = "Evaluation"
            }
        ]);
        CreateRepository();

        // Act
        var result = await _repository!.GetStatusById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEvaluationDoesNotExist_ReturnsNullAsync()
    {
        // Arrange
        SetupEvaluations(
        [
            new EvaluationEntity
        {
            Id = 1,
            Name = "Evaluation"
        }
        ]);

        var evaluationPolls = new List<EvaluationPollJoin>()
            .AsQueryable()
            .BuildMockDbSet();

        var pollInstances = new List<PollInstanceEntity>()
            .AsQueryable()
            .BuildMockDbSet();

        _mockContext
            .Setup(c => c.EvaluationPolls)
            .Returns(evaluationPolls.Object);

        _mockContext
            .Setup(c => c.PollInstances)
            .Returns(pollInstances.Object);

        CreateRepository();

        // Act
        var result = await _repository!.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEvaluationExists_ReturnsEvaluationAsync()
    {
        // Arrange
        var poll = new PollEntity
        {
            Uuid = "poll-1"
        };

        var evaluation = new EvaluationEntity
        {
            Id = 1,
            Name = "Evaluation",
            Status = "Completed"
        };

        SetupEvaluations([evaluation]);

        var evaluationPolls = new List<EvaluationPollJoin>
        {
            new()
            {
                EvaluationId = 1,
                Poll = poll
            }
        }
        .AsQueryable()
        .BuildMockDbSet();

        var pollInstances = new List<PollInstanceEntity>
        {
            new()
            {
                Uuid = "poll-1"
            },
            new()
            {
                Uuid = "other-poll"
            }
        }
        .AsQueryable()
        .BuildMockDbSet();

        _mockContext
            .Setup(c => c.EvaluationPolls)
            .Returns(evaluationPolls.Object);

        _mockContext
            .Setup(c => c.PollInstances)
            .Returns(pollInstances.Object);

        CreateRepository();

        // Act
        var result = await _repository!.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(1, result.Id);
        Assert.Equal("Evaluation", result.Name);
        Assert.Equal("Completed", result.Status);

        Assert.Single(result.Polls);
        Assert.Equal("poll-1", result.Polls.First().Uuid);

        Assert.Single(result.PollInstances);
        Assert.Equal("poll-1", result.PollInstances.First().Uuid);
    }

    [Fact]
    public async Task GetByDateRange_ReturnsEvaluationsWithinDateRangeAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        SetupEvaluations(
        [
            new EvaluationEntity
        {
            Id = 1,
            Name = "Inside",
            StartDate = new DateTime(2026, 1, 10)
        },
        new EvaluationEntity
        {
            Id = 2,
            Name = "Outside",
            StartDate = new DateTime(2026, 2, 10)
        }
        ]);

        var evaluationPolls = new List<EvaluationPollJoin>()
            .AsQueryable()
            .BuildMockDbSet();

        var pollInstances = new List<PollInstanceEntity>()
            .AsQueryable()
            .BuildMockDbSet();

        _mockContext
            .Setup(c => c.EvaluationPolls)
            .Returns(evaluationPolls.Object);

        _mockContext
            .Setup(c => c.PollInstances)
            .Returns(pollInstances.Object);

        CreateRepository();

        // Act
        var result = await _repository!.GetByDateRange(startDate, endDate);

        // Assert
        var evaluation = Assert.Single(result);

        Assert.Equal(1, evaluation.Id);
        Assert.Equal("Inside", evaluation.Name);
    }

    [Fact]
    public async Task GetByDateRange_IncludesBoundaryDatesAsync()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        SetupEvaluations(
        [
            new EvaluationEntity
        {
            Id = 1,
            Name = "Start",
            StartDate = startDate
        },
        new EvaluationEntity
        {
            Id = 2,
            Name = "End",
            StartDate = endDate
        }
        ]);

        var evaluationPolls = new List<EvaluationPollJoin>()
            .AsQueryable()
            .BuildMockDbSet();

        var pollInstances = new List<PollInstanceEntity>()
            .AsQueryable()
            .BuildMockDbSet();

        _mockContext
            .Setup(c => c.EvaluationPolls)
            .Returns(evaluationPolls.Object);

        _mockContext
            .Setup(c => c.PollInstances)
            .Returns(pollInstances.Object);

        CreateRepository();

        // Act
        var result = await _repository!.GetByDateRange(startDate, endDate);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByDateRange_WhenNoEvaluationsMatch_ReturnsEmptyAsync()
    {
        // Arrange
        SetupEvaluations(
        [
            new EvaluationEntity
            {
                Id = 1,
                Name = "Evaluation",
                StartDate = new DateTime(2026, 3, 1)
            }
        ]);

        var evaluationPolls = new List<EvaluationPollJoin>()
            .AsQueryable()
            .BuildMockDbSet();

        var pollInstances = new List<PollInstanceEntity>()
            .AsQueryable()
            .BuildMockDbSet();

        _mockContext
            .Setup(c => c.EvaluationPolls)
            .Returns(evaluationPolls.Object);

        _mockContext
            .Setup(c => c.PollInstances)
            .Returns(pollInstances.Object);

        CreateRepository();

        // Act
        var result = await _repository!.GetByDateRange(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 1, 31));

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task CountByDateRangeAsync_ReturnsNumberOfEvaluationsInRangeAsync()
    {
        // Arrange
        SetupEvaluations(
        [
            new EvaluationEntity
            {
                Id = 1,
                StartDate = new DateTime(2026, 1, 10)
            },
            new EvaluationEntity
            {
                Id = 2,
                StartDate = new DateTime(2026, 1, 20)
            },
            new EvaluationEntity
            {
                Id = 3,
                StartDate = new DateTime(2026, 2, 10)
            }
        ]);
        CreateRepository();

        // Act
        var result = await _repository!.CountByDateRangeAsync(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 1, 31));

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public async Task CountByDateRangeAsync_WhenNoEvaluationsMatch_ReturnsZeroAsync()
    {
        // Arrange
        SetupEvaluations(
        [
            new EvaluationEntity
            {
                Id = 1,
                StartDate = new DateTime(2026, 3, 10)
            }
        ]);

        CreateRepository();

        // Act
        var result = await _repository!.CountByDateRangeAsync(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 1, 31));

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetExpiredWithPendingStatusAsync_ReturnsMatchingStatusesBeforeDateAsync()
    {
        // Arrange
        var endDateBefore = new DateTime(2026, 2, 1);
        var evaluations = new List<EvaluationEntity>
        {
            new()
            {
                Id = 1,
                Name = "Expired Pending",
                Status = "Pending",
                EndDate = new DateTime(2026, 1, 10)
            },
            new()
            {
                Id = 2,
                Name = "Expired Completed",
                Status = "Completed",
                EndDate = new DateTime(2026, 1, 15)
            },
            new()
            {
                Id = 3,
                Name = "Not Expired",
                Status = "Pending",
                EndDate = new DateTime(2026, 3, 10)
            },
            new()
            {
                Id = 4,
                Name = "Wrong Status",
                Status = "Cancelled",
                EndDate = new DateTime(2026, 1, 10)
            }
        };
        SetupEvaluations(evaluations);
        CreateRepository();

        // Act
        var result = (await _repository!
            .GetExpiredWithPendingStatusAsync(["Pending", "Completed"], endDateBefore))
            .ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, e => e.Id == 1);
        Assert.Contains(result, e => e.Id == 2);
        Assert.DoesNotContain(result, e => e.Id == 3);
        Assert.DoesNotContain(result, e => e.Id == 4);
    }

    [Fact]
    public async Task GetExpiredWithPendingStatusAsync_WhenNothingMatches_ReturnsEmptyAsync()
    {
        // Arrange
        SetupEvaluations(
        [
            new EvaluationEntity
            {
                Id = 1,
                Name = "Evaluation",
                Status = "Completed",
                EndDate = new DateTime(2026, 3, 1)
            }
        ]);
        CreateRepository();

        // Act
        var result = await _repository!
            .GetExpiredWithPendingStatusAsync(["Pending"], new DateTime(2026, 1, 1));

        // Assert
        Assert.Empty(result);
    }
}
