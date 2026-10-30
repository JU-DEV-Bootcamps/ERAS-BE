using System.Reflection;

using Eras.Api.Controllers;
using Eras.Application.DTOs.Student;
using Eras.Application.Features.Students.Commands.CreateManualStudent;
using Eras.Application.Features.Students.Commands.DeleteStudent;
using Eras.Application.Features.Students.Commands.UpdateStudentProfile;
using Eras.Application.Features.Students.Queries.GetStudentProfile;
using Eras.Infrastructure.Authorization;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Api.Tests.Controllers;

public class StudentsControllerManualRegistrationTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly StudentsController _controller;

    public StudentsControllerManualRegistrationTests()
    {
        _controller = new StudentsController(_mediator.Object, new Mock<ILogger<StudentsController>>().Object);
    }

    private static StudentRegistrationDto Dto(int? StudentId = null) => new()
    {
        StudentId = StudentId,
        FirstName = "Ana",
        LastName = "Pérez",
        IdPassportNumber = "AB-1",
        PrimaryEmail = "ana@jala.university",
    };

    [Fact]
    public async Task CreateManualStudentAsync_ReturnsCreatedPointingToTheProfileRouteAsync()
    {
        StudentRegistrationDto input = Dto();
        StudentRegistrationDto created = Dto(StudentId: 55);
        _mediator
            .Setup(M => M.Send(It.Is<CreateManualStudentCommand>(C => C.Profile == input), default))
            .ReturnsAsync(created);

        IActionResult result = await _controller.CreateManualStudentAsync(input);

        CreatedAtRouteResult createdResult = Assert.IsType<CreatedAtRouteResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Same(created, createdResult.Value);
        Assert.Equal(55, createdResult.RouteValues!["Id"]);
    }

    [Fact]
    public async Task CreateManualStudentAsync_RouteNameMatchesARealRoute()
    {
        // ASP.NET strips the "Async" suffix from action names, so CreatedAtAction(nameof(...Async))
        // failed after the student had already been saved. The route is addressed by name instead;
        // this guards that the name used by the POST really exists on the GET profile action.
        _mediator
            .Setup(M => M.Send(It.IsAny<CreateManualStudentCommand>(), default))
            .ReturnsAsync(Dto(StudentId: 1));

        CreatedAtRouteResult result = Assert.IsType<CreatedAtRouteResult>(
            await _controller.CreateManualStudentAsync(Dto()));

        HttpGetAttribute? getProfile = typeof(StudentsController)
            .GetMethod(nameof(StudentsController.GetStudentProfileAsync))!
            .GetCustomAttribute<HttpGetAttribute>();
        Assert.NotNull(getProfile);
        Assert.Equal(result.RouteName, getProfile!.Name);
        Assert.Equal("{Id:int}/profile", getProfile.Template);
    }

    [Fact]
    public async Task GetStudentProfileAsync_ReturnsTheProfileAsync()
    {
        StudentRegistrationDto profile = Dto(StudentId: 7);
        _mediator
            .Setup(M => M.Send(It.Is<GetStudentProfileQuery>(Q => Q.StudentId == 7), default))
            .ReturnsAsync(profile);

        IActionResult result = await _controller.GetStudentProfileAsync(7);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(profile, ok.Value);
    }

    [Fact]
    public async Task UpdateStudentProfileAsync_ReturnsTheUpdatedProfileAsync()
    {
        StudentRegistrationDto input = Dto(StudentId: 7);
        StudentRegistrationDto updated = Dto(StudentId: 7);
        _mediator
            .Setup(M => M.Send(
                It.Is<UpdateStudentProfileCommand>(C => C.StudentId == 7 && C.Profile == input), default))
            .ReturnsAsync(updated);

        IActionResult result = await _controller.UpdateStudentProfileAsync(7, input);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(updated, ok.Value);
    }

    [Fact]
    public async Task GetStudentProfileAsync_ReturnsProfileViewFieldsAsync()
    {
        StudentRegistrationDto profile = Dto(StudentId: 7);
        profile.SisId = "STU-001";
        profile.Cgpa = 3.45m;
        profile.CurrentCourses = [new StudentCurrentCourseDto { Name = "Algorithms", Group = "G1", Section = "A" }];
        profile.IdentifiedRisks = ["Low attendance"];
        _mediator
            .Setup(M => M.Send(It.Is<GetStudentProfileQuery>(Q => Q.StudentId == 7), default))
            .ReturnsAsync(profile);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(await _controller.GetStudentProfileAsync(7));

        StudentRegistrationDto body = Assert.IsType<StudentRegistrationDto>(ok.Value);
        Assert.Equal("STU-001", body.SisId);
        Assert.Equal(3.45m, body.Cgpa);
        Assert.Single(body.CurrentCourses!);
        Assert.Null(body.SapStatus);
        Assert.Null(body.SupportReceived);
    }

    [Fact]
    public async Task UpdateStudentProfileAsync_ForwardsProfileViewFieldsAsync()
    {
        StudentRegistrationDto input = Dto(StudentId: 7);
        input.SapStatus = "Satisfactory";
        input.PassedCoursesCount = 12;
        input.SupportReceived = ["Tutoring"];
        _mediator
            .Setup(M => M.Send(It.IsAny<UpdateStudentProfileCommand>(), default))
            .ReturnsAsync(input);

        await _controller.UpdateStudentProfileAsync(7, input);

        _mediator.Verify(M => M.Send(
            It.Is<UpdateStudentProfileCommand>(C =>
                C.Profile.SapStatus == "Satisfactory"
                && C.Profile.PassedCoursesCount == 12
                && C.Profile.SupportReceived!.Single() == "Tutoring"),
            default), Times.Once);
    }

    [Fact]
    public async Task DeleteStudentAsync_ReturnsNoContentAsync()
    {
        IActionResult result = await _controller.DeleteStudentAsync(7);

        Assert.IsType<NoContentResult>(result);
        _mediator.Verify(M => M.Send(It.Is<DeleteStudentCommand>(C => C.StudentId == 7), default), Times.Once);
    }

    [Fact]
    public void Controller_RequiresAnyErasRoleAtClassLevel_AndEachActionRestrictsItself()
    {
        // ADR-0004: the class lets every ERAS role in; who may call each action is declared (and tested) per action,
        // see StudentsProfessionalAccessTests.
        AuthorizeAttribute? authorize = typeof(StudentsController).GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(authorize);
        Assert.Equal(ErasPolicies.AnyErasRole, authorize!.Policy);
    }
}
