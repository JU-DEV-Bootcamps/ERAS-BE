using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Polls.Queries.GetAllPollsQuery;
using Eras.Application.Features.Variables.Queries.GetByNameAndPollId;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Polls.Queries;
public class GetAllPollsQueryHandlerTest
{
    private readonly Mock<IPollRepository> _mockVariableRepository;
    private readonly Mock<ILogger<GetAllPollsQuery>> _mockLogger;
    private readonly GetAllPollsQueryHandler _handler;

    public GetAllPollsQueryHandlerTest()
    {
        _mockVariableRepository = new Mock<IPollRepository>();
        _mockLogger = new Mock<ILogger<GetAllPollsQuery>>();
        _handler = new GetAllPollsQueryHandler(_mockVariableRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_ResponseAsync()
    {
        // Arrange
        var query = new GetAllPollsQuery();
        var polls = new List<Poll>() {
            new Poll(),
            new Poll(),
        };

        _mockVariableRepository
            .Setup(Repo => Repo.GetAllAsync())
            .ReturnsAsync(polls);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
    }
}
