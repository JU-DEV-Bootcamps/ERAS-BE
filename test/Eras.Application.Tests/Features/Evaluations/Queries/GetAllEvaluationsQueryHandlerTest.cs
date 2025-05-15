using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Evaluations.Queries.GetAll;
using Eras.Application.Features.Polls.Queries.GetAllPollsQuery;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Evaluations.Queries;
public class GetAllEvaluationsQueryHandlerTest
{
    private readonly Mock<IEvaluationRepository> _mockEvaluationRepository;
    private readonly Mock<ILogger<GetAllEvaluationsQueryHandler>> _mockLogger;
    private readonly GetAllEvaluationsQueryHandler _handler;

    public GetAllEvaluationsQueryHandlerTest()
    {
        _mockEvaluationRepository = new Mock<IEvaluationRepository>();
        _mockLogger = new Mock<ILogger<GetAllEvaluationsQueryHandler>>();
        _handler = new GetAllEvaluationsQueryHandler(_mockEvaluationRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_ResponseAsync()
    {
        // Arrange
        var query = new GetAllEvaluationsQuery(new Utils.Pagination());
        var polls = new List<Poll>();
        var pollInstances = new List<PollInstance>();
        var evaluations = new List<Evaluation>() {
            new Evaluation(){ Name = "Evaluation", Status = "Active", 
                StartDate = DateTime.Now, EndDate = DateTime.Now, Polls = polls, 
                PollInstances = pollInstances },
            new Evaluation(){ Name = "Evaluation2", Status = "Active",
                StartDate = DateTime.Now, EndDate = DateTime.Now, Polls = polls,
                PollInstances = pollInstances },
        };

        _mockEvaluationRepository
            .Setup(Repo => Repo.GetAllAsync())
            .ReturnsAsync(evaluations);
        _mockEvaluationRepository.Setup(Repo => Repo.CountAsync()).ReturnsAsync(2);
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
    }
}
