using Eras.Application.Utils;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;

using Moq;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class ComponentsAvgRepositoryTests
{
    private readonly Mock<AppDbContext> _contextMock;
    private readonly Mock<IAnswerRiskValidator> _validatorMock;
    private readonly ComponentsAvgRepository _repository;

    public ComponentsAvgRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _contextMock = new Mock<AppDbContext>(options);
        _validatorMock = new Mock<IAnswerRiskValidator>();

        _repository = new ComponentsAvgRepository(
            _contextMock.Object,
            _validatorMock.Object);
    }

    private static Mock<DbSet<ErasCalculationsByPollEntity>> CreateMockErasCalculations(IEnumerable<ErasCalculationsByPollEntity> Data)
    {
        var queryable = new TestAsyncEnumerable<ErasCalculationsByPollEntity>(Data);
        var dbSet = new Mock<DbSet<ErasCalculationsByPollEntity>>();

        dbSet
            .As<IAsyncEnumerable<ErasCalculationsByPollEntity>>()
            .Setup(X => X.GetAsyncEnumerator(
                It.IsAny<CancellationToken>()))
            .Returns(() => queryable.GetAsyncEnumerator());

        dbSet
            .As<IQueryable<ErasCalculationsByPollEntity>>()
            .Setup(X => X.Provider)
            .Returns(((IQueryable<ErasCalculationsByPollEntity>)queryable).Provider);

        dbSet
            .As<IQueryable<ErasCalculationsByPollEntity>>()
            .Setup(X => X.Expression)
            .Returns(((IQueryable<ErasCalculationsByPollEntity>)queryable).Expression);

        dbSet
            .As<IQueryable<ErasCalculationsByPollEntity>>()
            .Setup(X => X.ElementType)
            .Returns(((IQueryable<ErasCalculationsByPollEntity>)queryable).ElementType);

        dbSet
            .As<IQueryable<ErasCalculationsByPollEntity>>()
            .Setup(X => X.GetEnumerator())
            .Returns(() => queryable.AsEnumerable().GetEnumerator());
        return dbSet;
    }

    private void SetupErasCalculations(
        IEnumerable<ErasCalculationsByPollEntity> Data)
    {
        var dbSet = CreateMockErasCalculations(Data);

        _contextMock
            .Setup(X => X.ErasCalculationsByPoll)
            .Returns(dbSet.Object);
    }

    [Fact]
    public async Task ComponentsAvgByStudent_ReturnsComponentAverageAsync()
    {
        var data = new[]
        {
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 2,
                AnswerText = "Low",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            },
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 4,
                AnswerText = "High",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            }
        };

        SetupErasCalculations(data);

        _validatorMock
            .Setup(X => X.IsValidAnswer(It.IsAny<string>()))
            .Returns(true);

        var result = await _repository.ComponentsAvgByStudent(1, 10);

        Assert.Single(result);

        var component = result[0];

        Assert.Equal(10, component.PollId);
        Assert.Equal(100, component.ComponentId);
        Assert.Equal("Engagement", component.Name);
        Assert.Equal(3f, component.ComponentAvg);
    }

    [Fact]
    public async Task ComponentsAvgByStudent_FiltersByStudentIdAsync()
    {
        var data = new[]
        {
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 2,
                AnswerText = "Answer 1",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            },
            new ErasCalculationsByPollEntity
            {
                StudentId = 2,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 10,
                AnswerText = "Answer 2",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            }
        };

        SetupErasCalculations(data);

        _validatorMock
            .Setup(X => X.IsValidAnswer(It.IsAny<string>()))
            .Returns(true);

        var result = await _repository.ComponentsAvgByStudent(1, 10);

        Assert.Single(result);

        Assert.Equal(2f, result[0].ComponentAvg);
    }

    [Fact]
    public async Task ComponentsAvgByStudent_FiltersByPollIdAsync()
    {
        var data = new[]
        {
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 2,
                AnswerText = "Answer 1",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            },
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 20,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 10,
                AnswerText = "Answer 2",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            }
        };

        SetupErasCalculations(data);

        _validatorMock
            .Setup(X => X.IsValidAnswer(It.IsAny<string>()))
            .Returns(true);

        var result = await _repository.ComponentsAvgByStudent(1, 10);

        Assert.Single(result);
        Assert.Equal(2f, result[0].ComponentAvg);
    }

    [Fact]
    public async Task ComponentsAvgByStudent_ExcludesInvalidAnswersFromAverageAsync()
    {
        var data = new[]
        {
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 2,
                AnswerText = "Valid",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            },
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 10,
                AnswerText = "Invalid",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            },
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 4,
                AnswerText = "Valid",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            }
        };

        SetupErasCalculations(data);

        _validatorMock
            .Setup(X => X.IsValidAnswer("Valid"))
            .Returns(true);

        _validatorMock
            .Setup(X => X.IsValidAnswer("Invalid"))
            .Returns(false);

        var result = await _repository.ComponentsAvgByStudent(1, 10);

        Assert.Single(result);
        Assert.Equal(3f, result[0].ComponentAvg);
    }

    [Fact]
    public async Task ComponentsAvgByStudent_CallsValidatorForEachAnswerAsync()
    {
        var data = new[]
        {
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 2,
                AnswerText = "Answer 1",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            },
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 4,
                AnswerText = "Answer 2",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            }
        };

        SetupErasCalculations(data);

        _validatorMock
            .Setup(X => X.IsValidAnswer(It.IsAny<string>()))
            .Returns(true);

        await _repository.ComponentsAvgByStudent(1, 10);

        _validatorMock.Verify(
            X => X.IsValidAnswer("Answer 1"), Times.Once);

        _validatorMock.Verify(
            X => X.IsValidAnswer("Answer 2"), Times.Once);
    }

    [Fact]
    public async Task ComponentsAvgByStudent_RoundsAverageToTwoDecimalPlacesAsync()
    {
        var data = new[]
        {
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 1,
                AnswerText = "Answer 1",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            },
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 2,
                AnswerText = "Answer 2",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            },
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 3,
                AnswerText = "Answer 3",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            }
        };

        SetupErasCalculations(data);

        _validatorMock
            .Setup(X => X.IsValidAnswer(It.IsAny<string>()))
            .Returns(true);

        var result = await _repository.ComponentsAvgByStudent(1, 10);

        Assert.Single(result);
        Assert.Equal(2f, result[0].ComponentAvg);
    }

    [Fact]
    public async Task ComponentsAvgByStudent_ReturnsMultipleComponentsAsync()
    {
        var data = new[]
        {
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 2,
                AnswerText = "Answer 1",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            },
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 4,
                AnswerText = "Answer 2",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            },
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 200,
                ComponentName = "Attendance",
                AnswerRisk = 5,
                AnswerText = "Answer 3",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            },
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 200,
                ComponentName = "Attendance",
                AnswerRisk = 3,
                AnswerText = "Answer 4",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            }
        };

        SetupErasCalculations(data);

        _validatorMock
            .Setup(X => X.IsValidAnswer(It.IsAny<string>()))
            .Returns(true);

        var result = await _repository.ComponentsAvgByStudent(1, 10);

        Assert.Equal(2, result.Count);

        var engagement = result.Single(X => X.ComponentId == 100);
        var attendance = result.Single(X => X.ComponentId == 200);

        Assert.Equal("Engagement", engagement.Name);
        Assert.Equal(3f, engagement.ComponentAvg);

        Assert.Equal("Attendance", attendance.Name);
        Assert.Equal(4f, attendance.ComponentAvg);
    }

    [Fact]
    public async Task ComponentsAvgByStudent_ReturnsEmptyList_WhenNoMatchingStudentAsync()
    {
        var data = new[]
        {
            new ErasCalculationsByPollEntity
            {
                StudentId = 2,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 5,
                AnswerText = "Answer",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            }
        };

        SetupErasCalculations(data);

        _validatorMock
            .Setup(X => X.IsValidAnswer(It.IsAny<string>()))
            .Returns(true);

        var result = await _repository.ComponentsAvgByStudent(1, 10);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ComponentsAvgByStudent_ReturnsEmptyList_WhenNoMatchingPollAsync()
    {
        var data = new[]
        {
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 20,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 5,
                AnswerText = "Answer",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            }
        };

        SetupErasCalculations(data);

        _validatorMock
            .Setup(X => X.IsValidAnswer(It.IsAny<string>()))
            .Returns(true);

        var result = await _repository.ComponentsAvgByStudent(1, 10);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ComponentsAvgByStudent_DoesNotMixDifferentComponentsWithSameStudentAndPollAsync()
    {
        var data = new[]
        {
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 100,
                ComponentName = "Engagement",
                AnswerRisk = 2,
                AnswerText = "Answer 1",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            },
            new ErasCalculationsByPollEntity
            {
                StudentId = 1,
                PollId = 10,
                ComponentId = 200,
                ComponentName = "Attendance",
                AnswerRisk = 8,
                AnswerText = "Answer 2",
                CohortName = "Cohort A",
                PollUuid = "12",
                Question = "first",
                StudentEmail = "sty@mail.com",
                StudentName = "Sty"
            }
        };

        SetupErasCalculations(data);

        _validatorMock
            .Setup(X => X.IsValidAnswer(It.IsAny<string>()))
            .Returns(true);

        var result = await _repository.ComponentsAvgByStudent(1, 10);

        Assert.Equal(2, result.Count);

        var engagement = result.Single(X => X.ComponentId == 100);
        var attendance = result.Single(X => X.ComponentId == 200);

        Assert.Equal(2f, engagement.ComponentAvg);
        Assert.Equal(8f, attendance.ComponentAvg);
    }
}
