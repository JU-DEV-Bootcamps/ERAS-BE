using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.PollVersions.Queries.GetByPollAndVersion;
using Eras.Application.Features.Variables.Queries.GetByname;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.PollVersions.Queries;
public class GetPollVersionByPollAndVersionQueryHandlerTest
{
    private readonly Mock<IPollVersionRepository> _mockPollVersionRepository;
    private readonly Mock<ILogger<GetPollVersionByPollAndVersionQueryHandler>> _mockLogger;
    private readonly GetPollVersionByPollAndVersionQueryHandler _handler;

    public GetPollVersionByPollAndVersionQueryHandlerTest()
    {
        _mockPollVersionRepository = new Mock<IPollVersionRepository>();
        _mockLogger = new Mock<ILogger<GetPollVersionByPollAndVersionQueryHandler>>();
        _handler = new GetPollVersionByPollAndVersionQueryHandler(_mockPollVersionRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_ResponseAsync()
    {
        // Arrange
        var query = new GetPollVersionByPollAndVersionQuery() { PollId = 2, VersionName = "VersionName"};
        var version = new PollVersion
        {
            Name = "VersionName",
            PollId = 1,
        };

        _mockPollVersionRepository
            .Setup(Repo => Repo.GetByPollAndVersionAsync(It.Is<string>(Name => Name == "VersionName"),
                It.Is<int>(Id => Id == 2)))
            .ReturnsAsync(version);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("VersionName", result.Body.Name);
    }
}
