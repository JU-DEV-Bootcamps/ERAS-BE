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
        _mockContext.Setup(r => r.Set<Assessment>()).Returns(mockSet.Object);

        // Act
        var result = await _repository.GetByStatusAsync(AssessmentStatus.Remitted);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, i => i.Id == 1);
        Assert.Contains(result, i => i.Id == 3);
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
        _mockContext.Setup(r => r.Set<Assessment>()).Returns(mockSet.Object);

        // Act
        var result = await _repository.GetByStatusAsync(AssessmentStatus.Finalized);

        // Assert
        Assert.Equal(0, result.Count());
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
        Intervention updated = await context.Set<Intervention>().FirstAsync(i => i.Id == intervention.Id);
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
        Intervention updated = await context.Set<Intervention>().FirstAsync(i => i.Id == intervention.Id);
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
        Intervention updated = await context.Set<Intervention>().FirstAsync(i => i.Id == intervention.Id);
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
        Intervention updated = await context.Set<Intervention>().FirstAsync(i => i.Id == intervention.Id);
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
}
