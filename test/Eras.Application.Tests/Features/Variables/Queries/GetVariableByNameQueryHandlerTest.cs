using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Components.Queries.GetByName;
using Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays;
using Eras.Application.Features.Variables.Queries.GetByname;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Variables.Queries;
public class GetVariableByNameQueryHandlerTest
{
    private readonly Mock<IVariableRepository> _mockVariableRepository;
    private readonly Mock<ILogger<GetVariableByNameQueryHandler>> _mockLogger;
    private readonly GetVariableByNameQueryHandler _handler;

    public GetVariableByNameQueryHandlerTest()
    {
        _mockVariableRepository = new Mock<IVariableRepository>();
        _mockLogger = new Mock<ILogger<GetVariableByNameQueryHandler>>();
        _handler = new GetVariableByNameQueryHandler(_mockVariableRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_ResponseAsync()
    {
        // Arrange
        var query = new GetVariableByNameQuery() { VariableName = "Variable" };
        var variable = new Variable()
        {
            Name = "Variable"
        };

        _mockVariableRepository
            .Setup(Repo => Repo.GetByNameAsync(It.Is<string>(Name => Name == "Variable")))
            .ReturnsAsync(variable);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Variable",result.Body.Name);
    }
}
