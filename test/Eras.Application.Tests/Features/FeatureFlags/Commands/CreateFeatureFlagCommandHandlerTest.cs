using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.FeatureFlags;
using Eras.Application.Features.FeatureFlags.Handlers.CommandHandlers;
using Eras.Application.Mappers;
using Eras.Domain.Common;
using Eras.Domain.Entities.FeatureFlagManagement;

using FluentValidation;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.FeatureFlags.Commands;

public class CreateFeatureFlagCommandHandlerTests
{
    private readonly Mock<IFeatureFlagRepository> _repositoryMock;
    private readonly Mock<IValidator<FeatureFlag>> _validatorMock;
    private readonly CreateFeatureFlagCommandHandler _handler;

    public CreateFeatureFlagCommandHandlerTests()
    {
        _repositoryMock = new Mock<IFeatureFlagRepository>();
        _validatorMock = new Mock<IValidator<FeatureFlag>>();
        _handler = new CreateFeatureFlagCommandHandler(
            _repositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateFeatureFlag_WhenFeatureFlagDoesNotExist()
    {
        // Arrange
        var featureFlag = CreateFeatureFlag(
            name: "NewFeature",
            id: 1,
            createdAt: DateTime.UtcNow.AddDays(-1),
            createdBy: "john.doe");

        var persistedEntity = CreateFeatureFlag(
            name: "NewFeature",
            id: 1,
            createdAt: featureFlag.Audit.CreatedAt,
            createdBy: featureFlag.Audit.CreatedBy);

        _repositoryMock
            .Setup(x => x.GetByNameAsync("NewFeature"))
            .ReturnsAsync((FeatureFlag?)null);

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<FeatureFlag>()))
            .ReturnsAsync(persistedEntity);

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<FeatureFlag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var request = new CreateFeatureFlagCommand(featureFlag.ToDTO());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(persistedEntity.Id, result.Id);
        Assert.Equal("NewFeature", result.Name);

        _repositoryMock.Verify(x => x.GetByNameAsync("NewFeature"), Times.Once);

        _repositoryMock.Verify(
            x => x.AddAsync(It.Is<FeatureFlag>(f => f.Name == "NewFeature" && f.Audit.CreatedBy == "john.doe")),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenFeatureFlagAlreadyExists()
    {
        // Arrange
        var existingEntity = CreateFeatureFlag(
            name: "ExistingFeature",
            id: 1);
        var featureFlagDto = new FeatureFlagDTO { Audit = new AuditInfo(), Description = "Description", Name = "NewFeature" };

        _repositoryMock
            .Setup(x => x.GetByNameAsync("ExistingFeature"))
            .ReturnsAsync(existingEntity);

        var request = new CreateFeatureFlagCommand(existingEntity.ToDTO());
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<FeatureFlag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(request, CancellationToken.None));

        // Assert
        Assert.Equal(
            "Feature flag ExistingFeature already exists.",exception.Message);

        _repositoryMock.Verify(
            x => x.AddAsync(It.IsAny<FeatureFlag>()),
            Times.Never);

        _validatorMock.Verify(
            x => x.ValidateAsync(
                It.IsAny<ValidationContext<FeatureFlag>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSetCreatedAtToUtcNow_WhenCreatedAtIsDefault()
    {
        // Arrange
        var featureFlag = CreateFeatureFlag(
            name: "NewFeature",
            id: 1,
            createdAt: default,
            createdBy: "john.doe");
       
        _repositoryMock
            .Setup(x => x.GetByNameAsync("NewFeature"))
            .ReturnsAsync((FeatureFlag?)null);

        FeatureFlag? addedEntity = null;

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<FeatureFlag>()))
            .Callback<FeatureFlag>(entity => addedEntity = entity)
            .ReturnsAsync((FeatureFlag entity) => entity);

        var before = DateTime.UtcNow;

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<FeatureFlag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var request = new CreateFeatureFlagCommand(featureFlag.ToDTO());

        // Act
        await _handler.Handle(request, CancellationToken.None);

        var after = DateTime.UtcNow;

        // Assert
        Assert.NotNull(addedEntity);
        Assert.NotEqual(default, addedEntity!.Audit.CreatedAt);
        Assert.InRange(addedEntity.Audit.CreatedAt, before, after);
    }

    [Fact]
    public async Task Handle_ShouldPreserveCreatedAt_WhenCreatedAtIsAlreadySet()
    {
        // Arrange
        var createdAt = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        var featureFlag = CreateFeatureFlag(
            name: "NewFeature",
            id: 1,
            createdAt: createdAt,
            createdBy: "john.doe");
        
        _repositoryMock
            .Setup(x => x.GetByNameAsync("NewFeature"))
            .ReturnsAsync((FeatureFlag?)null);

        FeatureFlag? addedEntity = null;

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<FeatureFlag>()))
            .Callback<FeatureFlag>(entity => addedEntity = entity)
            .ReturnsAsync((FeatureFlag entity) => entity);
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<FeatureFlag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var request = new CreateFeatureFlagCommand(featureFlag.ToDTO());

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(addedEntity);
        Assert.Equal(createdAt, addedEntity!.Audit.CreatedAt);
    }

    [Fact]
    public async Task Handle_ShouldSetCreatedByToSystem_WhenCreatedByIsEmpty()
    {
        // Arrange
        var featureFlag = CreateFeatureFlag(
            name: "NewFeature",
            id: 2,
            createdAt: DateTime.UtcNow,
            createdBy: "");
        
        _repositoryMock
            .Setup(x => x.GetByNameAsync("NewFeature"))
            .ReturnsAsync((FeatureFlag?)null);

        //SetupValidationSuccess();

        FeatureFlag? addedEntity = null;

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<FeatureFlag>()))
            .Callback<FeatureFlag>(entity => addedEntity = entity)
            .ReturnsAsync((FeatureFlag entity) => entity);
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<FeatureFlag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var request = new CreateFeatureFlagCommand(featureFlag.ToDTO());

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(addedEntity);
        Assert.Equal("System", addedEntity!.Audit.CreatedBy);
    }

    [Theory]
    [InlineData("john.doe")]
    [InlineData("admin")]
    public async Task Handle_ShouldPreserveCreatedBy_WhenCreatedByIsProvided(
        string createdBy)
    {
        // Arrange
        var featureFlag = CreateFeatureFlag(
            name: "NewFeature",
            id: 1,
            createdAt: DateTime.UtcNow,
            createdBy: createdBy);
        _repositoryMock
            .Setup(x => x.GetByNameAsync("NewFeature"))
            .ReturnsAsync((FeatureFlag?)null);

        FeatureFlag? addedEntity = null;

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<FeatureFlag>()))
            .Callback<FeatureFlag>(entity => addedEntity = entity)
            .ReturnsAsync((FeatureFlag entity) => entity);
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<FeatureFlag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var request = new CreateFeatureFlagCommand(featureFlag.ToDTO());

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(addedEntity);
        Assert.Equal(createdBy, addedEntity!.Audit.CreatedBy);
    }

    [Fact]
    public async Task Handle_ShouldSetDtoIdToNull_BeforePersisting()
    {
        // Arrange
        var featureFlag = CreateFeatureFlag(
            name: "NewFeature",
            id: 3,
            createdAt: DateTime.UtcNow,
            createdBy: "john.doe");
      
        _repositoryMock
            .Setup(x => x.GetByNameAsync("NewFeature"))
            .ReturnsAsync((FeatureFlag?)null);

        FeatureFlag? addedEntity = null;

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<FeatureFlag>()))
            .Callback<FeatureFlag>(entity => addedEntity = entity)
            .ReturnsAsync((FeatureFlag entity) => entity);
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<FeatureFlag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var request = new CreateFeatureFlagCommand(featureFlag.ToDTO());

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(addedEntity);
        Assert.NotEqual(featureFlag.Id, addedEntity!.Id);
    }

    private static FeatureFlag CreateFeatureFlag(
        string name,
        int id,
        DateTime? createdAt = null,
        string? createdBy = null)
    {
        return new FeatureFlag
        {
            Id = id,
            Name = name,
            Audit = new AuditInfo
            {
                CreatedAt = createdAt ?? default,
                CreatedBy = createdBy ?? ""
            }
        };
    }
}
