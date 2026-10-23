using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.FeatureFlags;
using Eras.Application.Features.FeatureFlags.Handlers.CommandHandlers;
using Eras.Application.Mappers;
using Eras.Domain.Common;
using Eras.Domain.Entities.FeatureFlagManagement;
using Eras.Error.Bussiness;

using FluentValidation;
using FluentValidation.Results;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;
namespace Eras.Application.Tests.Features.FeatureFlags.Commands;


public class UpdateFeatureFlagCommandHandlerTest
{
    private readonly Mock<IFeatureFlagRepository> _repositoryMock;
    private readonly Mock<IValidator<FeatureFlag>> _validatorMock;
    private readonly UpdateFeatureFlagCommandHandler _handler;

    public UpdateFeatureFlagCommandHandlerTest()
    {
        _repositoryMock = new Mock<IFeatureFlagRepository>();
        _validatorMock = new Mock<IValidator<FeatureFlag>>();
        _handler = new UpdateFeatureFlagCommandHandler(
            _repositoryMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateFeatureFlag_WhenEntityExists()
    {
        // Arrange
        var existingEntity = CreateFeatureFlag(1,"ExistingFeature");

        var requestDto = existingEntity.ToDTO();
        requestDto.Name = "UpdatedFeature";
        requestDto.Audit.ModifiedAt = DateTime.UtcNow.AddMinutes(-5);
        requestDto.Audit.ModifiedBy = "john.doe";

        var updatedEntity = CreateFeatureFlag(1, "UpdatedFeature");

        updatedEntity.Audit.ModifiedAt = requestDto.Audit.ModifiedAt;
        updatedEntity.Audit.ModifiedBy = requestDto.Audit.ModifiedBy;

        _repositoryMock
            .Setup(x => x.GetByIdNoTrackingAsync(1))
            .ReturnsAsync(existingEntity);

        SetupValidationSuccess();

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<FeatureFlag>()))
            .ReturnsAsync(updatedEntity);

        var request = new UpdateFeatureFlagCommand(requestDto);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("UpdatedFeature", result.Name);

        _repositoryMock.Verify(
            x => x.GetByIdNoTrackingAsync(1), Times.Once);

        _repositoryMock.Verify(
            x => x.UpdateAsync(It.Is<FeatureFlag>(entity =>
                entity.Id == 1 && entity.Name == "UpdatedFeature" && entity.Audit.ModifiedBy == "john.doe")),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenEntityDoesNotExist()
    {
        // Arrange
        var requestDto = CreateFeatureFlag(1, "MissingFeature").ToDTO();

        _repositoryMock
            .Setup(x => x.GetByIdNoTrackingAsync(1))
            .ReturnsAsync((FeatureFlag?)null);

        var request = new UpdateFeatureFlagCommand(requestDto);

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.StartsWith("Exception of type 'Eras.Error", exception.Message);

        _validatorMock.Verify(
            x => x.ValidateAsync(It.IsAny<FeatureFlag>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _repositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<FeatureFlag>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSetModifiedAt_WhenModifiedAtIsDefault()
    {
        // Arrange
        var existingEntity = CreateFeatureFlag(1, "Feature");

        var requestDto = existingEntity.ToDTO();
        requestDto.Audit.ModifiedAt = default;
        requestDto.Audit.ModifiedBy = "john.doe";

        _repositoryMock
            .Setup(x => x.GetByIdNoTrackingAsync(1))
            .ReturnsAsync(existingEntity);

        SetupValidationSuccess();

        FeatureFlag? updatedEntity = null;

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<FeatureFlag>()))
            .Callback<FeatureFlag>(entity => updatedEntity = entity)
            .ReturnsAsync((FeatureFlag entity) => entity);

        var request = new UpdateFeatureFlagCommand(requestDto);

        var before = DateTime.UtcNow;

        // Act
        await _handler.Handle(request, CancellationToken.None);

        var after = DateTime.UtcNow;

        // Assert
        Assert.NotNull(updatedEntity);
        Assert.NotEqual(default, updatedEntity!.Audit.ModifiedAt);
    }

    [Fact]
    public async Task Handle_ShouldPreserveModifiedAt_WhenModifiedAtIsProvided()
    {
        // Arrange
        var modifiedAt = new DateTime(2025, 5, 10, 12, 30, 0, DateTimeKind.Utc);

        var existingEntity = CreateFeatureFlag(1, "Feature");

        var requestDto = existingEntity.ToDTO();
        requestDto.Audit.ModifiedAt = modifiedAt;
        requestDto.Audit.ModifiedBy = "john.doe";

        _repositoryMock
            .Setup(x => x.GetByIdNoTrackingAsync(1))
            .ReturnsAsync(existingEntity);

        SetupValidationSuccess();

        FeatureFlag? updatedEntity = null;

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<FeatureFlag>()))
            .Callback<FeatureFlag>(entity => updatedEntity = entity)
            .ReturnsAsync((FeatureFlag entity) => entity);

        var request = new UpdateFeatureFlagCommand(requestDto);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(updatedEntity);
        Assert.Equal(modifiedAt, updatedEntity!.Audit.ModifiedAt);
    }

    [Fact]
    public async Task Handle_ShouldSetModifiedByToSystem_WhenModifiedByIsEmpty()
    {
        // Arrange
        var existingEntity = CreateFeatureFlag(1, "Feature");

        var requestDto = existingEntity.ToDTO();
        requestDto.Audit.ModifiedAt = DateTime.UtcNow;
        requestDto.Audit.ModifiedBy = "";

        _repositoryMock
            .Setup(x => x.GetByIdNoTrackingAsync(1))
            .ReturnsAsync(existingEntity);

        SetupValidationSuccess();

        FeatureFlag? updatedEntity = null;

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<FeatureFlag>()))
            .Callback<FeatureFlag>(entity => updatedEntity = entity)
            .ReturnsAsync((FeatureFlag entity) => entity);

        var request = new UpdateFeatureFlagCommand(requestDto);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(updatedEntity);
        Assert.Equal("System", updatedEntity!.Audit.ModifiedBy);
    }

    [Theory]
    [InlineData("john.doe")]
    [InlineData("admin")]
    public async Task Handle_ShouldPreserveModifiedBy_WhenModifiedByIsProvided(
        string modifiedBy)
    {
        // Arrange
        var existingEntity = CreateFeatureFlag(1, "Feature");

        var requestDto = existingEntity.ToDTO();
        requestDto.Audit.ModifiedAt = DateTime.UtcNow;
        requestDto.Audit.ModifiedBy = modifiedBy;

        _repositoryMock
            .Setup(x => x.GetByIdNoTrackingAsync(1))
            .ReturnsAsync(existingEntity);

        SetupValidationSuccess();

        FeatureFlag? updatedEntity = null;

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<FeatureFlag>()))
            .Callback<FeatureFlag>(entity => updatedEntity = entity)
            .ReturnsAsync((FeatureFlag entity) => entity);

        var request = new UpdateFeatureFlagCommand(requestDto);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(updatedEntity);
        Assert.Equal(modifiedBy, updatedEntity!.Audit.ModifiedBy);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
    {
        var existingEntity = CreateFeatureFlag(1, "Feature");

        var requestDto = existingEntity.ToDTO();

        _repositoryMock
            .Setup(x => x.GetByIdNoTrackingAsync(1))
            .ReturnsAsync(existingEntity);

        var validationFailure = new ValidationFailure("Name", "Feature flag name is required.");

        _validatorMock
            .Setup(x => x.ValidateAsync(
                It.IsAny<FeatureFlag>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ValidationResult(new[] { validationFailure }));

        var request = new UpdateFeatureFlagCommand(requestDto);

        await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(request, CancellationToken.None));

        _repositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<FeatureFlag>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationTokenToValidator()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        var existingEntity = CreateFeatureFlag(1, "Feature");

        var requestDto = existingEntity.ToDTO();

        _repositoryMock
            .Setup(x => x.GetByIdNoTrackingAsync(1))
            .ReturnsAsync(existingEntity);

        SetupValidationSuccess();

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<FeatureFlag>()))
            .ReturnsAsync(existingEntity);

        var request = new UpdateFeatureFlagCommand(requestDto);

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _validatorMock.Verify(
            x => x.ValidateAsync(It.IsAny<FeatureFlag>(), cancellationToken), Times.Once);
    }

    private void SetupValidationSuccess()
    {
        _validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<FeatureFlag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private static FeatureFlag CreateFeatureFlag(int id, string name)
    {
        return new FeatureFlag
        {
            Id = id,
            Name = name,
            Audit = new AuditInfo
            {
                ModifiedAt = DateTime.UtcNow,
                ModifiedBy = "test-user"
            }
        };
    }
}
