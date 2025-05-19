using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Variables.Queries.GetByName;
using Eras.Application.Features.Variables.Queries.GetByNameAndPollId;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Variables.Queries;
public class GetComponentByNameAndPollIdQueryHandlerTest
{
    private readonly Mock<IVariableRepository> _mockVariableRepository;
    private readonly Mock<ILogger<GetVariableByNameAndPollIdQueryHandler>> _mockLogger;
    private readonly GetVariableByNameAndPollIdQueryHandler _handler;

    public GetComponentByNameAndPollIdQueryHandlerTest()
    {
        _mockVariableRepository = new Mock<IVariableRepository>();
        _mockLogger = new Mock<ILogger<GetVariableByNameAndPollIdQueryHandler>>();
        _handler = new GetVariableByNameAndPollIdQueryHandler(_mockVariableRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_ResponseAsync()
    {
        // Arrange
        var query = new GetVariableByNameAndPollIdQuery() { VariableName = "Variable", PollId = 1 };
        var variable = new Variable()
        {
            Name = "Variable"
        };

        _mockVariableRepository
            .Setup(Repo => Repo.GetByNameAndPollIdAsync(It.Is<string>(Name => Name == "Variable"),
            It.Is<int>(Id => Id == 1)))
            .ReturnsAsync(variable);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Variable", result.Body.Name);
    }
}
