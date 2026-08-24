using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Features.FeatureFlags;
using Eras.Application.Features.FeatureFlags.Handlers.QueryHandlers;
using Eras.Domain.Common;
using Eras.Domain.Entities.FeatureFlagManagement;
using Eras.Error.Bussiness;

using Moq;

namespace Eras.Application.Tests.Features.FeatureFlags.Queries;

public class GetFeatureFlagByNameQueryHandlerTest
{
    private readonly Mock<IFeatureFlagRepository> _featureFlagRepositoryMock;
    private readonly GetFeatureFlagByNameQueryHandler _handler;

    public GetFeatureFlagByNameQueryHandlerTest()
    {
        _featureFlagRepositoryMock = new Mock<IFeatureFlagRepository>();
        _handler = new GetFeatureFlagByNameQueryHandler(_featureFlagRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldGetFeatureFlagByName_Successfully()
    {
        var featureFlag = new FeatureFlag
        {
            Name = "v1",
            Description = "Description",
            Audit = new AuditInfo { }
        };
        var request = new GetFeatureFlagByNameQuery("v1");

        _featureFlagRepositoryMock
            .Setup(x => x.GetByNameAsync("v1"))
            .ReturnsAsync(featureFlag);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(featureFlag.Name, result.Name);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenNameGivenNotExists()
    {
        var featureFlag = new FeatureFlag
        {
            Name = "v1",
            Description = "Description",
            Audit = new AuditInfo { }
        };
        var request = new GetFeatureFlagByNameQuery("v2");

        _featureFlagRepositoryMock
            .Setup(x => x.GetByNameAsync("v2"))
            .ReturnsAsync((FeatureFlag)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(request, CancellationToken.None));

        Assert.StartsWith("Exception of type 'Eras.Error", exception.Message);
        Assert.Equal(404, exception.StatusCode);
    }
}
