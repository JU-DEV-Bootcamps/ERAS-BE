using Eras.Application.DTOs;
using Eras.Application.Features.FeatureFlags;
using Eras.Application.Models;
using Eras.Application.Services;
using Eras.Error.Bussiness;

using MediatR;

using Moq;

using Xunit;
namespace Eras.Application.Tests.Services;

public class FeatureFlagServiceTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly FeatureFlagService _sut;

    public FeatureFlagServiceTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _sut = new FeatureFlagService(_mediatorMock.Object);
    }

    [Fact]
    public async Task UseEnhancedEvaluationImport_ShouldReturnTrue_WhenV2IsEnabled()
    {
        SetupFeatureFlag(FeatureFlags.Version2, isEnabled: true);

        var result = await _sut.UseEnhancedEvaluationImport();

        Assert.True(result);
        VerifyFeatureFlagRequested(FeatureFlags.Version2, Times.Once());
        VerifyFeatureFlagRequested(FeatureFlags.EnhancedEvaluationsImport, Times.Never());
    }

    [Fact]
    public async Task UseEnhancedEvaluationImport_ShouldCheckEnhancedImport_WhenV2IsDisabled()
    {
        SetupFeatureFlag(FeatureFlags.Version2, isEnabled: false);
        SetupFeatureFlag(FeatureFlags.EnhancedEvaluationsImport, isEnabled: true);

        var result = await _sut.UseEnhancedEvaluationImport();

        Assert.True(result);
        VerifyFeatureFlagRequested(FeatureFlags.Version2, Times.Once());
        VerifyFeatureFlagRequested(FeatureFlags.EnhancedEvaluationsImport, Times.Once());
    }

    [Fact]
    public async Task UseEnhancedEvaluationImport_ShouldReturnFalse_WhenBothFlagsAreDisabled()
    {
        SetupFeatureFlag(FeatureFlags.Version2, isEnabled: false);
        SetupFeatureFlag(FeatureFlags.EnhancedEvaluationsImport, isEnabled: false);

        var result = await _sut.UseEnhancedEvaluationImport();

        Assert.False(result);
        VerifyFeatureFlagRequested(FeatureFlags.Version2, Times.Once());
        VerifyFeatureFlagRequested(FeatureFlags.EnhancedEvaluationsImport, Times.Once());
    }

    [Fact]
    public async Task UseEnhancedEvaluationImport_ShouldReturnFalse_WhenV2FlagDoesNotExist()
    {
        SetupFeatureFlag(FeatureFlags.Version2, featureFlag: null);
        SetupFeatureFlag(FeatureFlags.EnhancedEvaluationsImport, isEnabled: false);

        var result = await _sut.UseEnhancedEvaluationImport();

        Assert.False(result);
        VerifyFeatureFlagRequested(FeatureFlags.Version2, Times.Once());
        VerifyFeatureFlagRequested(FeatureFlags.EnhancedEvaluationsImport, Times.Once());
    }

    [Fact]
    public async Task UseEnhancedEvaluationImport_ShouldReturnTrue_WhenV2ThrowsNotFoundException_AndEnhancedImportIsEnabled()
    {
        _mediatorMock
            .Setup(x => x.Send(
                It.Is<GetFeatureFlagByNameQuery>(query => query.Name == FeatureFlags.Version2),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("message"));
        SetupFeatureFlag(FeatureFlags.EnhancedEvaluationsImport, isEnabled: true);

        var result = await _sut.UseEnhancedEvaluationImport();

        Assert.True(result);
        VerifyFeatureFlagRequested(FeatureFlags.Version2, Times.Once());
        VerifyFeatureFlagRequested(FeatureFlags.EnhancedEvaluationsImport, Times.Once());
    }

    [Fact]
    public async Task UseEnhancedEvaluationImport_ShouldReturnFalse_WhenEnhancedImportFlagDoesNotExist()
    {
        SetupFeatureFlag(FeatureFlags.Version2, isEnabled: false);
        SetupFeatureFlag(FeatureFlags.EnhancedEvaluationsImport, featureFlag: null);

        var result = await _sut.UseEnhancedEvaluationImport();

        Assert.False(result);
    }

    [Fact]
    public async Task UseEnhancedEvaluationImport_ShouldReturnFalse_WhenEnhancedImportThrowsNotFoundException()
    {
        SetupFeatureFlag(FeatureFlags.Version2, isEnabled: false);
        _mediatorMock
            .Setup(x => x.Send(
                It.Is<GetFeatureFlagByNameQuery>(query => query.Name == FeatureFlags.EnhancedEvaluationsImport),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("messages"));

        var result = await _sut.UseEnhancedEvaluationImport();

        Assert.False(result);
        VerifyFeatureFlagRequested(FeatureFlags.Version2, Times.Once());
        VerifyFeatureFlagRequested(FeatureFlags.EnhancedEvaluationsImport, Times.Once());
    }

    [Fact]
    public async Task IsV2Enabled_ShouldReturnTrue_WhenV2IsEnabled()
    {
        SetupFeatureFlag(FeatureFlags.Version2, isEnabled: true);
        var result = await _sut.IsV2Enabled();

        // Assert
        Assert.True(result);

        VerifyFeatureFlagRequested(FeatureFlags.Version2, Times.Once());
    }

    [Fact]
    public async Task IsV2Enabled_ShouldReturnFalse_WhenV2IsDisabled()
    {
        SetupFeatureFlag(FeatureFlags.Version2, isEnabled: false);

        var result = await _sut.IsV2Enabled();

        Assert.False(result);

        VerifyFeatureFlagRequested(FeatureFlags.Version2, Times.Once());
    }

    [Fact]
    public async Task IsV2Enabled_ShouldReturnFalse_WhenV2FlagDoesNotExist()
    {
        SetupFeatureFlag(FeatureFlags.Version2, featureFlag: null);

        var result = await _sut.IsV2Enabled();

        Assert.False(result);
        VerifyFeatureFlagRequested(FeatureFlags.Version2, Times.Once());
    }

    private void SetupFeatureFlag(
        string featureFlagName,
        bool isEnabled)
    {
        SetupFeatureFlag(
            featureFlagName,
            new FeatureFlagDTO
            {
                IsEnabled = isEnabled,
                Name = "feature",
                Audit = new Domain.Common.AuditInfo(),
                Description = "Description",
            });
    }

    private void SetupFeatureFlag(
        string featureFlagName,
        FeatureFlagDTO? featureFlag)
    {
        _mediatorMock
            .Setup(x => x.Send(
                It.Is<GetFeatureFlagByNameQuery>(query => query.Name == featureFlagName),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(featureFlag);
    }

    private void VerifyFeatureFlagRequested(
        string featureFlagName,
        Times times)
    {
        _mediatorMock.Verify(
            x => x.Send(
                It.Is<GetFeatureFlagByNameQuery>(query => query.Name == featureFlagName),
                It.IsAny<CancellationToken>()),
            times);
    }
}

