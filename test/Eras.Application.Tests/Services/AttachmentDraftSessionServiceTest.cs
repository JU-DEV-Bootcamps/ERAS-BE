using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.AttachmentManagement;
using Eras.Application.Services;
using Eras.Domain.Entities;

using Moq;

namespace Eras.Application.Tests.Services;

public class AttachmentDraftSessionServiceTest
{
    private readonly Mock<IAttachmentDraftSessionRepository> _mockRepository;
    private readonly AttachmentDraftSessionService _service;

    public AttachmentDraftSessionServiceTest()
    {
        _mockRepository = new Mock<IAttachmentDraftSessionRepository>();
        _service = new AttachmentDraftSessionService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateDraftSessionAsync_Should_PersistASessionScopedToTheCallerAsync()
    {
        // Arrange
        _mockRepository
            .Setup(X => X.AddAsync(It.Is<AttachmentDraftSession>(S => S.CreatedBy == "user-1")))
            .ReturnsAsync((AttachmentDraftSession Session) => new AttachmentDraftSession
            {
                Id = 42,
                CreatedBy = Session.CreatedBy,
                CreatedAt = Session.CreatedAt
            });

        // Act
        DraftSessionDto result = await _service.CreateDraftSessionAsync("user-1");

        // Assert
        Assert.Equal(42, result.DraftId);
        _mockRepository.Verify(X => X.AddAsync(It.Is<AttachmentDraftSession>(S => S.CreatedBy == "user-1")), Times.Once);
    }

    [Fact]
    public async Task CreateDraftSessionAsync_Should_ReturnTheRepositoryAssignedIdAsDraftIdAsync()
    {
        // Arrange
        _mockRepository
            .Setup(X => X.AddAsync(It.IsAny<AttachmentDraftSession>()))
            .ReturnsAsync((AttachmentDraftSession Session) => new AttachmentDraftSession
            {
                Id = 7,
                CreatedBy = Session.CreatedBy,
                CreatedAt = Session.CreatedAt
            });

        // Act
        DraftSessionDto result = await _service.CreateDraftSessionAsync("user-2");

        // Assert
        Assert.Equal(7, result.DraftId);
    }
}
