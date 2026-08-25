using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.Services;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Services;

public class InterventionAttachmentMigrationServiceTest
{
    private readonly Mock<IAssessmentRepository> _mockAssessmentRepository;
    private readonly Mock<IAttachmentRepository> _mockAttachmentRepository;
    private readonly InterventionAttachmentMigrationService _service;

    public InterventionAttachmentMigrationServiceTest()
    {
        _mockAssessmentRepository = new Mock<IAssessmentRepository>();
        _mockAttachmentRepository = new Mock<IAttachmentRepository>();
        _service = new InterventionAttachmentMigrationService(
            _mockAssessmentRepository.Object,
            _mockAttachmentRepository.Object,
            Mock.Of<ILogger<InterventionAttachmentMigrationService>>());
    }

    private static GroupIntervention BuildIntervention(int id, IReadOnlyCollection<string> paths, IReadOnlyCollection<string> hashes) => new()
    {
        Id = id,
        DateUtc = DateTime.UtcNow,
        StudentIds = [1],
        Attachments = paths,
        AttachmentHashes = hashes
    };

    private void SetupAssessments(params Intervention[] interventions)
    {
        var assessment = new Assessment
        {
            Id = 1,
            CreatedBy = "x",
            Service = "x",
            Status = AssessmentStatus.Remitted,
            StudentIds = [1],
            Interventions = interventions
        };
        _mockAssessmentRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Assessment> { assessment });
    }

    private void SetupNoExistingAttachments() =>
        _mockAttachmentRepository
            .Setup(x => x.GetByEntityAsync(InterventionConstants.AttachmentEntityType, It.IsAny<int>()))
            .ReturnsAsync(new List<Attachment>());

    [Fact]
    public async Task MigrateAsync_Should_CreateOneAttachmentPerPathHashPairAsync()
    {
        // Arrange
        var intervention = BuildIntervention(1, ["interventions/1/a.pdf", "interventions/1/b.pdf"], ["hash-a", "hash-b"]);
        SetupAssessments(intervention);
        SetupNoExistingAttachments();

        // Act
        var result = await _service.MigrateAsync();

        // Assert
        Assert.Equal(1, result.InterventionsProcessed);
        Assert.Equal(2, result.AttachmentsCreated);
        Assert.True(result.IsValid);
        _mockAttachmentRepository.Verify(x => x.AddAsync(It.Is<Attachment>(a =>
            a.EntityType == InterventionConstants.AttachmentEntityType
            && a.EntityId == 1
            && a.StorageKey == "interventions/1/a.pdf"
            && a.ContentHash == "hash-a"
            && a.OriginalFileName == "a.pdf" // Path.GetFileName(path) — the GUID-based name path itself carried, not the true original name
            && a.MimeType == "application/pdf" // ContentTypeResolver.Resolve(path) — extension is trustworthy even though the base name isn't
            && a.SizeBytes == null)), Times.Once);
        _mockAttachmentRepository.Verify(x => x.AddAsync(It.Is<Attachment>(a => a.StorageKey == "interventions/1/b.pdf")), Times.Once);
    }

    [Fact]
    public async Task MigrateAsync_Should_SkipInterventionsWithNoAttachmentsAsync()
    {
        // Arrange
        var intervention = BuildIntervention(1, [], []);
        SetupAssessments(intervention);

        // Act
        var result = await _service.MigrateAsync();

        // Assert
        Assert.Equal(0, result.InterventionsProcessed);
        Assert.Equal(0, result.AttachmentsCreated);
        _mockAttachmentRepository.Verify(x => x.AddAsync(It.IsAny<Attachment>()), Times.Never);
    }

    [Fact]
    public async Task MigrateAsync_Should_BeIdempotent_SkippingAlreadyMigratedHashesAsync()
    {
        // Arrange
        var intervention = BuildIntervention(1, ["interventions/1/a.pdf", "interventions/1/b.pdf"], ["hash-a", "hash-b"]);
        SetupAssessments(intervention);
        _mockAttachmentRepository
            .Setup(x => x.GetByEntityAsync(InterventionConstants.AttachmentEntityType, 1))
            .ReturnsAsync(new List<Attachment>
            {
                new() { EntityType = InterventionConstants.AttachmentEntityType, EntityId = 1, StorageKey = "interventions/1/a.pdf", ContentHash = "hash-a", CreatedBy = "legacy-migration" }
            });

        // Act — a second run after "hash-a" was already migrated
        var result = await _service.MigrateAsync();

        // Assert: only the un-migrated "hash-b" gets created
        Assert.Equal(1, result.AttachmentsCreated);
        Assert.True(result.IsValid);
        _mockAttachmentRepository.Verify(x => x.AddAsync(It.Is<Attachment>(a => a.ContentHash == "hash-a")), Times.Never);
        _mockAttachmentRepository.Verify(x => x.AddAsync(It.Is<Attachment>(a => a.ContentHash == "hash-b")), Times.Once);
    }

    [Fact]
    public async Task MigrateAsync_Should_SkipAndReport_WhenArraysHaveMismatchedLengthsAsync()
    {
        // Arrange — corrupt data: 2 paths but only 1 hash
        var intervention = BuildIntervention(1, ["interventions/1/a.pdf", "interventions/1/b.pdf"], ["hash-a"]);
        SetupAssessments(intervention);

        // Act
        var result = await _service.MigrateAsync();

        // Assert
        Assert.Contains(1, result.InterventionsSkippedDueToMismatchedArrays);
        Assert.False(result.IsValid);
        _mockAttachmentRepository.Verify(x => x.AddAsync(It.IsAny<Attachment>()), Times.Never);
    }

    [Fact]
    public async Task MigrateAsync_Should_ReportValidationFailure_When_ProjectedCountDoesNotMatchExpectedAsync()
    {
        // Arrange — an existing Attachment row whose hash ("stale-hash") doesn't correspond to
        // anything in the current array (e.g. left over from a prior partial/inconsistent state).
        // That inflates the projected post-migration count above what the array actually expects.
        var intervention = BuildIntervention(1, ["interventions/1/a.pdf", "interventions/1/b.pdf"], ["hash-a", "hash-b"]);
        SetupAssessments(intervention);
        _mockAttachmentRepository
            .Setup(x => x.GetByEntityAsync(InterventionConstants.AttachmentEntityType, 1))
            .ReturnsAsync(new List<Attachment>
            {
                new() { EntityType = InterventionConstants.AttachmentEntityType, EntityId = 1, StorageKey = "x", ContentHash = "hash-a", CreatedBy = "legacy-migration" },
                new() { EntityType = InterventionConstants.AttachmentEntityType, EntityId = 1, StorageKey = "y", ContentHash = "stale-hash", CreatedBy = "legacy-migration" }
            });

        // Act
        var result = await _service.MigrateAsync();

        // Assert: expected 2 (array length), but projected is 3 (2 pre-existing distinct hashes + 1 newly created "hash-b")
        var failure = Assert.Single(result.ValidationFailures);
        Assert.Equal(1, failure.InterventionId);
        Assert.Equal(2, failure.ExpectedCount);
        Assert.Equal(3, failure.ActualCount);
        Assert.False(result.IsValid);
    }
}
