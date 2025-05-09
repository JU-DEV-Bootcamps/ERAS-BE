using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.PollVersions.Queries.GetAllByPoll;
using Eras.Application.Features.PollVersions.Queries.GetByPollAndVersion;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.PollVersions.Queries;
public class GetAllPollVersionByPollQueryHandlerTest
{
    private readonly Mock<IPollVersionRepository> _mockPollVersionRepository;
    private readonly Mock<ILogger<GetAllPollVersionByPollQueryHandler>> _mockLogger;
    private readonly GetAllPollVersionByPollQueryHandler _handler;

    public GetAllPollVersionByPollQueryHandlerTest()
    {
        _mockPollVersionRepository = new Mock<IPollVersionRepository>();
        _mockLogger = new Mock<ILogger<GetAllPollVersionByPollQueryHandler>>();
        _handler = new GetAllPollVersionByPollQueryHandler(_mockPollVersionRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_ResponseAsync()
    {
        // Arrange
        var query = new GetAllPollVersionByPollQuery() { PollId = 1};
        var versions = new List<PollVersion>() {
            new PollVersion
            {
                Name = "VersionName",
                PollId = 1,
            },
            new PollVersion
            {
                Name = "VersionName2",
                PollId = 1,
            }
        };

        _mockPollVersionRepository
            .Setup(Repo => Repo.GetAllByPollAsync(It.Is<int>(Id => Id == 1)))
            .ReturnsAsync(versions);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2,result.Body.Count);
    }
}
