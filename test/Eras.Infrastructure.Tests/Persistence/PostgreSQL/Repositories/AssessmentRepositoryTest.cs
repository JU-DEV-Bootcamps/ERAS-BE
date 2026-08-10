using Eras.Domain.Entities.AssessmentManagement;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories.AssessmentManagement;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
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

    private static TestIntervention BuildIntervention(int Id, params int[] StudentIds)
    {
        return new TestIntervention
        {
            Id = Id,
            DateUtc = DateTime.UtcNow,
            StudentIds = StudentIds,
            Mode = InterventionMode.InPlace
        };
    }

    private void SetupAssessments(params Assessment[] Assessments)
    {
        var mockSet = Assessments.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(C => C.Set<Assessment>()).Returns(mockSet.Object);
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnListAsync()
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
        };

        var mockSet = assessments.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(R => R.Set<Assessment>()).Returns(mockSet.Object);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, I => I.Id == 1);
        Assert.Contains(result, I => I.Id == 2);
    }

    [Fact]
    public async Task GetByStudentIdAsync_Should_ReturnListAsync()
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
                StudentIds = [1, 20]
            },
        };

        var mockSet = assessments.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(R => R.Set<Assessment>()).Returns(mockSet.Object);

        // Act
        var result = await _repository.GetByStudentIdAsync(1);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, I => I.Id == 1);
        Assert.Contains(result, I => I.Id == 3);
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
        Assert.Contains(result.Result, I => I.Id == 1);
        Assert.Contains(result.Result, I => I.Id == 2);
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
    public async Task GetInterventionsContainingStudent_Should_ReturnEmpty_When_StudentIdsListIsEmptyAsync()
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
    public async Task GetByStatus_Should_ReturnListofRemittedAsync()
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
        _mockContext.Setup(R => R.Set<Assessment>()).Returns(mockSet.Object);

        // Act
        var result = await _repository.GetByStatusAsync(AssessmentStatus.Remitted);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, I => I.Id == 1);
        Assert.Contains(result, I => I.Id == 3);
    }

    [Fact]
    public async Task GetByStatus_Should_ReturnEmptyListAsync()
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
        _mockContext.Setup(R => R.Set<Assessment>()).Returns(mockSet.Object);

        // Act
        var result = await _repository.GetByStatusAsync(AssessmentStatus.Finalized);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddAttachmentsAsync_Should_SaveAttachmentAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);

        var intervention = new TestIntervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1, 2],
            Mode = InterventionMode.InPlace,
            Attachments = new List<string>().AsReadOnly(),
            AttachmentHashes = new List<string>().AsReadOnly()
        };

        var assessment = new Assessment
        {
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = "Any",
            Service = "Smth",
            StudentIds = [1, 2],
            Status = AssessmentStatus.InProgress,
            Interventions = new List<Intervention> { intervention }
        };

        context.Set<Assessment>().Add(assessment);
        await context.SaveChangesAsync();
        var repository = new AssessmentRepository(context, _mockLogger.Object);

        var newPaths = new List<string> { "interventions/1/file1.pdf" };
        var newHashes = new List<string> { "HASH123" };

        // Act
        await repository.AddAttachmentsAsync(intervention.Id, newPaths, newHashes);

        // Assert
        Intervention updated = await context.Set<Intervention>().FirstAsync(I => I.Id == intervention.Id);
        Assert.Single(updated.Attachments);
        Assert.Contains("interventions/1/file1.pdf", updated.Attachments);
        Assert.Single(updated.AttachmentHashes);
        Assert.Contains("HASH123", updated.AttachmentHashes);
    }

    [Fact]
    public async Task AddAttachmentsAsync_Should_ThrowKeyNotFoundException_When_InterventionNotFoundAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);
        var repository = new AssessmentRepository(context, _mockLogger.Object);

        // Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => repository.AddAttachmentsAsync(999, ["interventions/999/file.pdf"], ["HASH"]));

        Assert.Equal("Intervention '999' not found.", exception.Message);
    }

    [Fact]
    public async Task RemoveAttachmentAsync_Should_RemoveMatchingAttachmentAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);

        var intervention = new TestIntervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1, 2],
            Mode = InterventionMode.InPlace,
            Attachments = new List<string>
        {
            "interventions/1/file1.pdf",
            "interventions/1/file2.png"
        }.AsReadOnly(),
            AttachmentHashes = new List<string> { "HASH1", "HASH2" }.AsReadOnly()
        };

        var assessment = new Assessment
        {
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = "Any",
            Service = "Smth",
            StudentIds = [1, 2],
            Status = AssessmentStatus.InProgress,
            Interventions = new List<Intervention> { intervention }
        };

        context.Set<Assessment>().Add(assessment);
        await context.SaveChangesAsync();

        var repository = new AssessmentRepository(context, _mockLogger.Object);

        // Act
        await repository.RemoveAttachmentAsync(intervention.Id, "interventions/1/file1.pdf");

        // Assert
        Intervention updated = await context.Set<Intervention>().FirstAsync(I => I.Id == intervention.Id);
        Assert.Single(updated.Attachments);
        Assert.Contains("interventions/1/file2.png", updated.Attachments);
        Assert.Single(updated.AttachmentHashes);
        Assert.Contains("HASH2", updated.AttachmentHashes);
    }

    [Fact]
    public async Task RemoveAttachmentAsync_Should_MatchByFileName_CaseInsensitiveAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);

        var intervention = new TestIntervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1, 2],
            Mode = InterventionMode.InPlace,
            Attachments = new List<string> { "interventions/1/File1.PDF" }.AsReadOnly(),
            AttachmentHashes = new List<string> { "HASH1" }.AsReadOnly()
        };

        var assessment = new Assessment
        {
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = "Any",
            Service = "Smth",
            StudentIds = [1, 2],
            Status = AssessmentStatus.InProgress,
            Interventions = new List<Intervention> { intervention }
        };

        context.Set<Assessment>().Add(assessment);
        await context.SaveChangesAsync();

        var repository = new AssessmentRepository(context, _mockLogger.Object);

        // Act
        await repository.RemoveAttachmentAsync(intervention.Id, "somewhere-else/file1.pdf");

        // Assert
        Intervention updated = await context.Set<Intervention>().FirstAsync(I => I.Id == intervention.Id);
        Assert.Empty(updated.Attachments);
        Assert.Empty(updated.AttachmentHashes);
    }

    [Fact]
    public async Task RemoveAttachmentAsync_Should_NotModifyList_When_FileNameNotFoundAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);

        var intervention = new TestIntervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1, 2],
            Mode = InterventionMode.InPlace,
            Attachments = new List<string> { "interventions/1/file1.pdf" }.AsReadOnly(),
            AttachmentHashes = new List<string> { "HASH1" }.AsReadOnly()
        };

        var assessment = new Assessment
        {
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = "Any",
            Service = "Smth",
            StudentIds = [1, 2],
            Status = AssessmentStatus.InProgress,
            Interventions = new List<Intervention> { intervention }
        };

        context.Set<Assessment>().Add(assessment);
        await context.SaveChangesAsync();

        var repository = new AssessmentRepository(context, _mockLogger.Object);

        // Act
        await repository.RemoveAttachmentAsync(intervention.Id, "does-not-exist.pdf");

        // Assert 
        Intervention updated = await context.Set<Intervention>().FirstAsync(I => I.Id == intervention.Id);
        Assert.Single(updated.Attachments);
        Assert.Contains("interventions/1/file1.pdf", updated.Attachments);
        Assert.Single(updated.AttachmentHashes);
        Assert.Contains("HASH1", updated.AttachmentHashes);
    }

    [Fact]
    public async Task RemoveAttachmentAsync_Should_ThrowException_When_InterventionNotFoundAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);
        var repository = new AssessmentRepository(context, _mockLogger.Object);

        // Act 
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => repository.RemoveAttachmentAsync(999, "interventions/999/file.pdf"));
        //Assert
        Assert.Equal("Intervention '999' not found.", exception.Message);
    }

    [Fact]
    public async Task GetAttachmentHashesAsync_Should_ReturnHashes_When_InterventionExistsAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);

        var intervention = new TestIntervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1, 2],
            Mode = InterventionMode.InPlace,
            Attachments = new List<string> { "interventions/1/file1.pdf", "interventions/1/file2.png" }.AsReadOnly(),
            AttachmentHashes = new List<string> { "HASH1", "HASH2" }.AsReadOnly()
        };

        var assessment = new Assessment
        {
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = "Any",
            Service = "Smth",
            StudentIds = [1, 2],
            Status = AssessmentStatus.InProgress,
            Interventions = new List<Intervention> { intervention }
        };

        context.Set<Assessment>().Add(assessment);
        await context.SaveChangesAsync();

        var repository = new AssessmentRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAttachmentHashesAsync(intervention.Id, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains("HASH1", result);
        Assert.Contains("HASH2", result);
    }

    [Fact]
    public async Task GetAttachmentHashesAsync_Should_ReturnEmpty_When_InterventionNotFoundAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);
        var repository = new AssessmentRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAttachmentHashesAsync(999, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task DeleteAssessmentAsync_Should_DeleteAssessmentAndInterventionsAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new AppDbContext(options);
        var assessment = new Assessment
        {
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = "Any",
            Service = "Service",
            StudentIds = [1],
            Status = AssessmentStatus.Remitted
        };
        context.Set<Assessment>().Add(assessment);
        await context.SaveChangesAsync();
        var intervention = new TestIntervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1],
            Mode = InterventionMode.InPlace
        };
        context.Interventions.Add(intervention);
        context.Entry(intervention).Property("remission_id").CurrentValue = assessment.Id;
        await context.SaveChangesAsync();
        var repository = new AssessmentRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteAssessmentAsync(assessment.Id);

        // Assert
        Assert.Empty(context.Set<Assessment>());
        Assert.Empty(context.Interventions);
    }

    [Fact]
    public async Task DeleteAssessmentAsync_Should_Throw_WhenAssessmentIsNotFoundOrNotRemittedAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new AppDbContext(options);
        context.Set<Assessment>().Add(new Assessment
        {
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = "Any",
            Service = "Service",
            StudentIds = [1],
            Status = AssessmentStatus.InProgress
        });
        await context.SaveChangesAsync();
        var repository = new AssessmentRepository(context, _mockLogger.Object);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => repository.DeleteAssessmentAsync(1));

        // Assert
        Assert.Equal(
            "Assessment '1' not found or not permitted.",
            exception.Message);
    }

    [Fact]
    public async Task GetByIdWithInterventionsAsync_Should_ReturnAssessmentAndInterventionsAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new AppDbContext(options);
        var assessment = new Assessment
        {
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = "Any",
            Service = "Service",
            StudentIds = [1],
            Status = AssessmentStatus.Remitted
        };
        context.Set<Assessment>().Add(assessment);
        await context.SaveChangesAsync();
        var intervention = new TestIntervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1],
            Mode = InterventionMode.InPlace
        };
        context.Interventions.Add(intervention);
        context.Entry(intervention).Property("remission_id").CurrentValue = assessment.Id;
        await context.SaveChangesAsync();
        var repository = new AssessmentRepository(context, _mockLogger.Object);

        // Act
        await repository.GetByIdWithInterventionsAsync(assessment.Id);

        // Assert
        Assert.NotEmpty(context.Set<Assessment>());
        Assert.NotEmpty(context.Interventions);
    }

    [Fact]
    public async Task DeleteInterventionAsync_Should_Throw_WhenInterventionIsNotFoundAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new AppDbContext(options);
        context.Set<Assessment>().Add(new Assessment
        {
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = "Any",
            Service = "Service",
            StudentIds = [1],
            Status = AssessmentStatus.InProgress
        });
        await context.SaveChangesAsync();
        var repository = new AssessmentRepository(context, _mockLogger.Object);

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => repository.DeleteInterventionAsync(1,1));

        // Assert
        Assert.Equal(
            "Intervention '1' not found for assessment '1'.",
            exception.Message);
    }

    [Fact]
    public async Task DeleteInterventionAsync_ShouldThrow_InterventionNotFoundAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new AppDbContext(options);
        var assessment = new Assessment
        {
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = "Any",
            Service = "Service",
            StudentIds = [1],
            Status = AssessmentStatus.Remitted
        };
        context.Set<Assessment>().Add(assessment);
        await context.SaveChangesAsync();
        var intervention = new TestIntervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1],
            Mode = InterventionMode.InPlace
        };
        context.Interventions.Add(intervention);
        context.Entry(intervention).Property("remission_id").CurrentValue = assessment.Id;
        await context.SaveChangesAsync();
        var repository = new AssessmentRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteInterventionAsync(assessment.Id, 1);

        // Assert
        Assert.Empty(context.Interventions);
    }

    [Fact]
    public async Task ReplaceInterventionsAsync_ShouldReplaceExistingInterventionsAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);

        var assessment = new Assessment
        {
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = "Any",
            Service = "Service",
            StudentIds = [1],
            Status = AssessmentStatus.InProgress
        };

        context.Set<Assessment>().Add(assessment);
        await context.SaveChangesAsync();

        var existingIntervention = new TestIntervention
        {
            DateUtc = DateTime.UtcNow.AddDays(-1),
            StudentIds = [1],
            Mode = InterventionMode.InPlace
        };

        context.Interventions.Add(existingIntervention);
        context.Entry(existingIntervention)
            .Property("remission_id")
            .CurrentValue = assessment.Id;

        await context.SaveChangesAsync();

        var newInterventions = new List<Intervention>
        {
            new TestIntervention
            {
                DateUtc = DateTime.UtcNow,
                StudentIds = [1, 2],
                Mode = InterventionMode.InPlace
            },
            new TestIntervention
            {
                DateUtc = DateTime.UtcNow,
                StudentIds = [2],
                Mode = InterventionMode.InPlace
            }
        };

        var repository = new AssessmentRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ReplaceInterventionsAsync(
            assessment.Id,
            newInterventions);

        // Assert
        Assert.Same(newInterventions, result);

        var interventions = await context.Interventions.ToListAsync();

        Assert.Equal(2, interventions.Count);
        Assert.DoesNotContain(
            interventions,
            I => I.Id == existingIntervention.Id);
    }

    [Fact]
    public async Task AddInterventionAsync_ShouldAddInterventionAndSetAssessmentIdAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);

        var intervention = new TestIntervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1, 2],
            Mode = InterventionMode.InPlace
        };

        var repository = new AssessmentRepository(
            context,
            _mockLogger.Object);

        // Act
        var result = await repository.AddInterventionAsync(
            123,
            intervention);

        // Assert
        Assert.Same(intervention, result);
        var saved = await context.Interventions
            .FirstAsync(I => I.Id == intervention.Id);
        Assert.Equal(intervention.Id, saved.Id);
    }

    [Fact]
    public async Task GetInterventionByIdAsync_ShouldReturnIntervention_WhenExistsAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);

        var intervention = new TestIntervention
        {
            DateUtc = DateTime.UtcNow,
            StudentIds = [1, 2],
            Mode = InterventionMode.InPlace
        };

        context.Interventions.Add(intervention);
        await context.SaveChangesAsync();

        var repository = new AssessmentRepository(
            context,
            _mockLogger.Object);

        // Act
        var result = await repository.GetInterventionByIdAsync(intervention.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(intervention.Id, result.Id);
    }
}
