using Eras.Domain.Entities.AssessmentManagement;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories.AssessmentManagement;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

using MockQueryable.Moq;

using Moq;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class AssessmentRepositoryTest
{
    private Mock<AppDbContext> _mockContext;
    private readonly Mock<ILogger<AssessmentRepository>> _mockLogger;
    private AssessmentRepository _repository;

    public AssessmentRepositoryTest()
    {
        _mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        _mockLogger = new Mock<ILogger<AssessmentRepository>>();
        _repository = new AssessmentRepository(_mockContext.Object, _mockLogger.Object);
    }

    private static TestIntervention BuildIntervention(int id, params int[] studentIds)
    {
        return new TestIntervention
        {
            Id = id,
            DateUtc = DateTime.UtcNow,
            StudentIds = studentIds,
            Mode = InterventionMode.InPlace
        };
    }

    private void SetupAssessments(params Assessment[] assessments)
    {
        var mockSet = assessments.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Set<Assessment>()).Returns(mockSet.Object);
    }

    [Fact]
    public void GetInterventionsContainingStudent_Should_Return()
    {
        // Arrange
        var interventions = new List<Intervention>
        {
            BuildIntervention(1, 1, 2),
            BuildIntervention(2, 3, 4)
        };
        
        var assessment = new Assessment
        {
            Id = 1,
            CreatedBy = "Any",
            Service = "Smth",
            Status = AssessmentStatus.InProgress,
            StudentIds = [1, 2, 3, 4],
            Interventions = interventions
        };
        SetupAssessments(assessment);

        // Act
        var result = _repository.GetInterventionsContainingStudentAsync(assessment, [1]);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Result);
        Assert.Equal(1, result.Result.First().Id);
    }

    [Fact]
    public void GetInterventionsContainingStudent_ShouldReturn_MultipleMatches()
    {
        // Arrange
        var interventions = new List<Intervention>
        {
            BuildIntervention(1, 1, 2),
            BuildIntervention(2, 2, 3),
            BuildIntervention(3, 4, 5)
        };

        var assessment = new Assessment
        {
            Id = 1,
            CreatedBy = "Any",
            Service = "Smth",
            Status = AssessmentStatus.InProgress,
            StudentIds = [1, 2, 3],
            Interventions = interventions
        };

        SetupAssessments(assessment);

        // Act
        var result = _repository.GetInterventionsContainingStudentAsync(assessment, [2]);

        // Assert
        Assert.Equal(2, result.Result.Count());
        Assert.Contains(result.Result, i => i.Id == 1);
        Assert.Contains(result.Result, i => i.Id == 2);
    }

    [Fact]
    public void GetInterventionsContainingAnyStudent_Should_ReturnEmpty_When_NoMatch()
    {
        // Arrange
        var interventions = new List<Intervention>
        {
            BuildIntervention(1, 1, 2)
        };
        var assessment = new Assessment
        {
            Id = 1,
            CreatedBy = "Any",
            Service = "Smth",
            Status = AssessmentStatus.Remitted,
            StudentIds = [1, 2, 3],
            Interventions = interventions
        };

        SetupAssessments(assessment);

        // Act
        var result = _repository.GetInterventionsContainingStudentAsync(assessment, [3]);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Result);
    }

    [Fact]
    public void GetInterventionsContainingStudentAsync_Should_ReturnEmpty_When_AssessmentHasNoInterventions()
    {
        // Arrange
        var assessment = new Assessment
        {
            Id = 1,
            CreatedBy = "Any",
            Service = "Smth",
            Status = AssessmentStatus.Remitted,
            StudentIds = [1, 2],
            Interventions = new List<Intervention>()
        };

        SetupAssessments(assessment);

        // Act
        var result = _repository.GetInterventionsContainingStudentAsync(assessment, [1]);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Result);
    }

    [Fact]
    public async Task GetInterventionsContainingStudent_Should_ReturnEmpty_When_StudentIdsListIsEmpty()
    {
        // Arrange
        var interventions = new List<Intervention> { BuildIntervention(1, 1, 2) };
        var assessment = new Assessment
        {
            Id = 1,
            CreatedBy = "Any",
            Service = "Smth",
            Status = AssessmentStatus.Remitted,
            StudentIds = [1, 2],
            Interventions = interventions
        };

        SetupAssessments(assessment);

        // Act
        var result = await _repository.GetInterventionsContainingStudentAsync(assessment, []);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByStatus_Should_ReturnListofRemitted()
    {
        // Arrange
        var assessments = new List<Assessment>
        {
            new Assessment {
                 Id = 1,
                CreatedBy = "",
                Service = "",
                Status = AssessmentStatus.Remitted,
                StudentIds = [1, 2]
            },
            new Assessment {
                 Id = 2,
                CreatedBy = "",
                Service = "",
                Status = AssessmentStatus.InProgress,
                StudentIds = [5, 8]
            },
            new Assessment {
                 Id = 3,
                CreatedBy = "",
                Service = "",
                Status = AssessmentStatus.Remitted,
                StudentIds = [10, 20]
            },
        };

        var mockSet = assessments.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(r => r.Set<Assessment>()).Returns(mockSet.Object);

        // Act
        var result = await _repository.GetByStatusAsync(AssessmentStatus.Remitted);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, i => i.Id == 1);
        Assert.Contains(result, i => i.Id == 3);
    }

    [Fact]
    public async Task GetByStatus_Should_ReturnEmptyList()
    {
        // Arrange
        var assessments = new List<Assessment>
        {
            new Assessment {
                 Id = 1,
                CreatedBy = "",
                Service = "",
                Status = AssessmentStatus.InProgress,
                StudentIds = [1, 2]
            },
            new Assessment {
                 Id = 2,
                CreatedBy = "",
                Service = "",
                Status = AssessmentStatus.InProgress,
                StudentIds = [5, 8]
            },
            new Assessment {
                 Id = 3,
                CreatedBy = "",
                Service = "",
                Status = AssessmentStatus.Remitted,
                StudentIds = [10, 20]
            },
        };

        var mockSet = assessments.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(r => r.Set<Assessment>()).Returns(mockSet.Object);

        // Act
        var result = await _repository.GetByStatusAsync(AssessmentStatus.Finalized);

        // Assert
        Assert.Equal(0, result.Count());
        Assert.Empty(result);
    }
}
