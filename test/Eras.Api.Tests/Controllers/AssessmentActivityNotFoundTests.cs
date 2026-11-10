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

/// <summary>The status code a client gets when the activity history of an unknown assessment is requested.</summary>
public class AssessmentActivityNotFoundTests
{
    [Fact]
    [Trait("Change", "681-add-activity-history-to-assessment-details")]
    [Trait("AC", "681-add-activity-history-to-assessment-details/AC-12")]
    public async Task GetAssessmentActivity_UnknownAssessment_RespondsNotFoundAsync()
    {
        // Arrange (GIVEN) a user with an ERAS role and no assessment with id 999999,
        // with the real handler behind the controller and the global ErrorFilter in front of it
        var assessmentRepository = new Mock<IAssessmentRepository>();
        assessmentRepository.Setup(R => R.GetByIdAsync(999999)).ReturnsAsync((Assessment?)null);
        var activityRepository = new Mock<IAssessmentActivityRepository>();
        var handler = new GetAssessmentActivityQueryHandler(
            assessmentRepository.Object,
            activityRepository.Object,
            Mock.Of<IMapper<AssessmentActivity, AssessmentActivityDto>>());

        var mediator = new Mock<IMediator>();
        mediator
            .Setup(M => M.Send(It.IsAny<GetAssessmentActivityQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetAssessmentActivityQuery Query, CancellationToken Token) => handler.Handle(Query, Token));
        var controller = new AssessmentsController(
            mediator.Object,
            Mock.Of<IFileStorageService>(),
            Mock.Of<ICurrentUserService>());
        var filter = new ErrorFilter(Mock.Of<ILogger<Exception>>());

        // Act (WHEN) they request the activity history of assessment 999999
        Exception? thrown = null;
        try
        {
            await controller.GetAssessmentActivity(999999, CancellationToken.None);
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

        // Assert (THEN) the API responds 404 Not Found and no event data is returned
        var result = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        activityRepository.Verify(
            R => R.GetByAssessmentIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
