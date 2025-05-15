using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Polls.Queries.GetAllPollsQuery;
using Eras.Application.Features.Polls.Queries.GetPollByName;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Polls.Queries;
public class GetPollByNameQueryHandlerTest
{
    private readonly Mock<IPollRepository> _mockVariableRepository;
    private readonly Mock<ILogger<GetPollByNameQuery>> _mockLogger;
    private readonly GetPollByNameQueryHandler _handler;

    public GetPollByNameQueryHandlerTest()
    {
        _mockVariableRepository = new Mock<IPollRepository>();
        _mockLogger = new Mock<ILogger<GetPollByNameQuery>>();
        _handler = new GetPollByNameQueryHandler(_mockVariableRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_ResponseAsync()
    {
        // Arrange
        var query = new GetPollByNameQuery() { pollName = "Poll1" };
        var variables = new List<Variable>();
        var components = new List<Component>() { 
            new Component(){ Name = "Component", Variables = variables}        
        };
        var poll = new Poll() { Name = "Poll1", LastVersionDate = DateTime.Now ,
            Components = components};

        _mockVariableRepository
            .Setup(Repo => Repo.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(poll);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal("Poll1",result.Body.Name);
    }
}
