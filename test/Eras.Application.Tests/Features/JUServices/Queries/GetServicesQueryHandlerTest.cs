using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.JUServices.Queries.GetJUServices;
using Eras.Application.Utils;
using Eras.Domain.Common;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.JUServices.Queries;
public class GetJUServicesQueryHandlerTests
{
    private readonly Mock<IJUServiceRepository> _mockJuServiceRepository;
    private readonly Mock<ILogger<GetJUServicesQuery>> _mockLogger;
    private readonly GetJUServicesQueryHandler _handler;

    public GetJUServicesQueryHandlerTests()
    {
        _mockJuServiceRepository = new Mock<IJUServiceRepository>();
        _mockLogger = new Mock<ILogger<GetJUServicesQuery>>();
        _handler = new GetJUServicesQueryHandler(_mockJuServiceRepository.Object, _mockLogger.Object);
    }

    private static JUService BuildJUService(int Id = 1, string Name = "Test Service") => new() { Id = Id, Name = Name, Audit = new AuditInfo() };

    [Fact]
    public async Task Handler_ShouldReturnAllServicesAndTotalCount()
    {
        var services = new List<JUService>
        {
            BuildJUService(),
            BuildJUService(2, "Another test")
        };
        var query = new GetJUServicesQuery();

        _mockJuServiceRepository.Setup(Repo => Repo.GetAllAsync()).ReturnsAsync(services);
        _mockJuServiceRepository.Setup(Repo => Repo.CountAsync()).ReturnsAsync(services.Count);

        PagedResult<JUService> result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(services.Count, result.Count);
        Assert.Equal(services.Count, result.Items.Count);
        Assert.IsType<List<JUService>>(result.Items);
    }

    [Fact]
    public async Task Handler_ShouldHandleExceptionAndReturnEmptyPagedResult()
    {
        var query = new GetJUServicesQuery();
        _mockJuServiceRepository.Setup(Repo => Repo.GetAllAsync())
            .ThrowsAsync(new Exception("Error querying JU Services."));
        
        PagedResult<JUService> result = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(0, result.Count);
        Assert.Empty(result.Items);
    }
}