using Eras.Api.Controllers.AssessmentManagement;
using Eras.Api.Filters;
using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Api.Tests.Controllers;

/// <summary>The status code a client gets when the activity history of an unknown intervention is requested.</summary>
public class InterventionActivityNotFoundTests
{
    private static async Task<(ObjectResult Result, Mock<IInterventionActivityRepository> Activity)> RequestAsync(
        Assessment? Assessment, int AssessmentId, int InterventionId)
    {
        var assessmentRepository = new Mock<IAssessmentRepository>();
        assessmentRepository.Setup(R => R.GetByIdWithInterventionsAsync(AssessmentId)).ReturnsAsync(Assessment);
        var activityRepository = new Mock<IInterventionActivityRepository>();
        var handler = new GetInterventionActivityQueryHandler(
            assessmentRepository.Object,
            activityRepository.Object,
            Mock.Of<IMapper<InterventionActivity, InterventionActivityDto>>());

        var mediator = new Mock<IMediator>();
        mediator
            .Setup(M => M.Send(It.IsAny<GetInterventionActivityQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetInterventionActivityQuery Query, CancellationToken Token) => handler.Handle(Query, Token));
        var controller = new AssessmentsController(
            mediator.Object,
            Mock.Of<IFileStorageService>(),
            Mock.Of<ICurrentUserService>());
        var filter = new ErrorFilter(Mock.Of<ILogger<Exception>>());

        Exception? thrown = null;
        try
        {
            await controller.GetInterventionActivity(AssessmentId, InterventionId, CancellationToken.None);
        }
        catch (Exception Ex)
        {
            thrown = Ex;
        }
        var context = new ActionExecutedContext(
            new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
            new List<IFilterMetadata>(),
            controller)
        {
            Exception = thrown,
        };
        filter.OnActionExecuted(context);

        return (Assert.IsType<ObjectResult>(context.Result), activityRepository);
    }

    [Fact]
    [Trait("Change", "680-add-activity-history-to-intervention-details")]
    [Trait("AC", "680-add-activity-history-to-intervention-details/AC-12")]
    public async Task GetInterventionActivity_UnknownIntervention_RespondsNotFoundAsync()
    {
        // Arrange (GIVEN) an assessment without an intervention with id 999999
        var assessment = new Assessment
        {
            Id = 1,
            Interventions = [new Intervention { Id = 5, DateUtc = DateTime.UtcNow, StudentIds = [] }],
            CreatedBy = "auto",
            Service = "Any",
            Status = AssessmentStatus.Remitted,
            StudentIds = [1],
        };

        // Act (WHEN) the activity history of intervention 999999 is requested
        var (result, activity) = await RequestAsync(assessment, 1, 999999);

        // Assert (THEN) the API responds 404 Not Found and no event data is returned
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        activity.Verify(
            R => R.GetByInterventionIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetInterventionActivity_UnknownAssessment_RespondsNotFoundAsync()
    {
        var (result, activity) = await RequestAsync(null, 999999, 5);

        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        activity.Verify(
            R => R.GetByInterventionIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
