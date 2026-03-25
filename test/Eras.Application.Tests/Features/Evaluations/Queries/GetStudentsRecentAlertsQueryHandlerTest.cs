using Castle.Core.Logging;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.EvaluationDetails.Queries.GetStudentsRecentAlerts;
using Eras.Application.Features.Evaluations.Queries.GetAll;
using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;
using Eras.Domain.Common;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Eras.Application.Tests.Features.Evaluations.Queries;

public class GetStudentsRecentAlertsQueryHandlerTest
{
    private readonly Mock<IErasEvaluationDetailsViewRepository> _mockRepository;
    private readonly Mock<ILogger<GetStudentsRecentAlertsQueryHandler>> _mockLogger;
    private readonly GetStudentsRecentAlertsQueryHandler _handler;

    public GetStudentsRecentAlertsQueryHandlerTest()
    {
        _mockRepository =  new Mock<IErasEvaluationDetailsViewRepository>();
        _mockLogger = new Mock<ILogger<GetStudentsRecentAlertsQueryHandler>>();
        _handler = new GetStudentsRecentAlertsQueryHandler( _mockRepository.Object, _mockLogger.Object );
    }

    [Fact]
    public async Task Handle_Should_Return_Success_ResponseAsync()
    {
        // Arrange
        var query = new GetStudentsRecentAlertsQuery(new Utils.Pagination());
        var polls = new List<Poll>();
        var pollInstances = new List<PollInstance>();
        var recentAlerts = new List<GetStudentsRecentAlertsResponse>() {
            new GetStudentsRecentAlertsResponse(){
                StudentId= "0",
                StudentName= "Fake user",
                RiskLevel= Models.Enums.RiskLevelEnum.RiskLevel.High,
                Category= "Familiar",
                Date= DateTime.Now,
                Status="Pending",
            },
            new GetStudentsRecentAlertsResponse()
            {
                StudentId= "1",
                StudentName= "Fake student",
                RiskLevel= Models.Enums.RiskLevelEnum.RiskLevel.Medium,
                Category= "Familiar",
                Date= DateTime.Now,
                Status="Pending",
            },
        };

        _mockRepository
            .Setup(Repo => Repo.GetRecentAlertsStudentAsync(0,4))
            .ReturnsAsync(recentAlerts);
        _mockRepository.Setup(Repo => Repo.CountRecentAlerts()).ReturnsAsync(2);
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
    }
}
