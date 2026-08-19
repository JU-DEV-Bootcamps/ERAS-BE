using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Evaluations.Queries;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Evaluations.Queries;
public class GetEvaluationProcessSummaryQueryHandlerTest
{
    private readonly Mock<IEvaluationRepository> _evaluationRepository;
    private readonly Mock<ILogger<GetEvaluationProcessSummaryQueryHandler>> _logger;
    private readonly GetEvaluationProcessSummaryQueryHandler _handler;

    public GetEvaluationProcessSummaryQueryHandlerTest()
    {
        _evaluationRepository = new Mock<IEvaluationRepository>();
        _logger = new Mock<ILogger<GetEvaluationProcessSummaryQueryHandler>>();
        _handler = new GetEvaluationProcessSummaryQueryHandler(
            _evaluationRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundResponse_WhenEvaluationDoesNotExist()
    {
        // Arrange
        var request = new GetEvaluationSummaryQuery
        {
            EvaluationId = 1
        };

        _evaluationRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Evaluation?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Evaluation not found", result.Message);
        Assert.False(result.Success);

        _evaluationRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEvaluationResponse_WhenEvaluationExists()
    {
        // Arrange
        var evaluationId = Guid.NewGuid();

        var evaluation = new Evaluation
        {
            Id = 1,
            Status = "Complete"
        };

        var request = new GetEvaluationSummaryQuery
        {
            EvaluationId = 1
        };

        _evaluationRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(evaluation);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Same(evaluation, result.Body);
        Assert.Equal("Evaluation Complete", result.Message);
        Assert.True(result.Success);

        _evaluationRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
    }
}
