using System.Reflection.Metadata;

using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.FeatureFlags;
using Eras.Application.Features.FeatureFlags.Handlers.CommandHandlers;
using Eras.Application.Models;
using Eras.Domain.Common;
using Eras.Domain.Entities.FeatureFlagManagement;

using FluentValidation;

using Moq;

namespace Eras.Application.Tests.Features.FeatureFlags.Commands;

public class DeleteFeatureFlagCommandHandlerTest
{
    private readonly Mock<IFeatureFlagRepository> _repositoryMock;
    private readonly DeleteFeatureFlagCommandHandler _handler;
    
    public DeleteFeatureFlagCommandHandlerTest()
    {
        _repositoryMock = new Mock<IFeatureFlagRepository>();
        _handler = new DeleteFeatureFlagCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteFeatureFlag_Successfully()
    {
        var featureFlag = new FeatureFlag
        {
            Name = "v1",
            Description = "Description",
            Audit = new AuditInfo { }
        };
        var command = new DeleteFeatureFlagCommand(1);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(featureFlag);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(
            x => x.GetByIdAsync(1),
            Times.Once);

        _repositoryMock.Verify(
            x => x.DeleteAsync(featureFlag),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenFeatureFlagIsNotFound()
    {
        var command = new DeleteFeatureFlagCommand(1);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((FeatureFlag?)null!);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Feature Flag 1 not found.", exception.Message);

        _repositoryMock.Verify(
            x => x.GetByIdAsync(1),
            Times.Once);

        _repositoryMock.Verify(
            x => x.DeleteAsync(It.IsAny<FeatureFlag>()),
            Times.Never);
    }
}
