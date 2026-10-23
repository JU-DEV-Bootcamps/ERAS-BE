using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Evaluations.Queries.GetByDateRange;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Evaluations.Queries;

public class GetEvaluationsByDateRangeQueryHandlerTests
{
    private readonly Mock<IEvaluationRepository> _evaluationRepository;
    private readonly Mock<ILogger<GetEvaluationsByDateRangeQueryHandler>> _logger;
    private readonly GetEvaluationsByDateRangeQueryHandler _handler;

    public GetEvaluationsByDateRangeQueryHandlerTests()
    {
        _evaluationRepository = new Mock<IEvaluationRepository>();
        _logger = new Mock<ILogger<GetEvaluationsByDateRangeQueryHandler>>();
        _handler = new GetEvaluationsByDateRangeQueryHandler(
            _evaluationRepository.Object, _logger.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEvaluations_WhenRepositoryReturnsEvaluations()
    {
        // Arrange
        var startDate = new DateTime(2026, 1, 1);
        var endDate = new DateTime(2026, 1, 31);

        var evaluations = new List<Evaluation>
        {
            new Evaluation
            {
                Id = 1,
                Name = "Evaluation 1"
            },
            new Evaluation
            {
                Id = 2,
                Name = "Evaluation 2"
            }
        };

        var request = new GetEvaluationsByDateRangeQuery
        {
            StartDate = startDate,
            EndDate = endDate
        };

        _evaluationRepository
            .Setup(x => x.GetByDateRange(startDate, endDate))
            .ReturnsAsync(evaluations);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Same(evaluations, result);

        _evaluationRepository.Verify(x => x.GetByDateRange(startDate, endDate), Times.Once);
    }
}
