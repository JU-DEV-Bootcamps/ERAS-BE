using Eras.Api.Controllers;
using Eras.Application.Contracts.Infrastructure;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Features.ErasUsers;
using Eras.Application.Features.ErasUsers.Models;
using Eras.Domain.Entities.UserManagement;
using Eras.Error.Bussiness;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace Eras.Api.Tests.Controllers;

/// <summary>
/// Status codes of the professional profile endpoints (603-persist-professional-profile). Helper tests: they
/// carry no AC tag because the ACs are already tagged on the handler and policy tests.
/// </summary>
public class UsersControllerProfessionalProfileTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly UsersController _controller;

    public UsersControllerProfessionalProfileTests()
    {
        _currentUserServiceMock.Setup(C => C.Email).Returns("pro@test.com");
        _currentUserServiceMock.Setup(C => C.Sub).Returns("sub-pro");
        _controller = new UsersController(_mediatorMock.Object, _currentUserServiceMock.Object, new KeycloakRoleNames());
    }

    private static ProfessionalProfileDTO CreateProfile() => new()
    {
        Position = "Dean",
        Faculty = "Engineering",
        OfficeHours = "Mon 9:00-11:00",
        SubjectsTaught = ["Algorithms"],
        Courses = [new ProfessionalCourseDTO("CS101", "Intro to Programming")],
    };

    [Fact]
    public async Task GetMyProfessionalProfileAsync_ProfileExists_Returns200WithProfileAsync()
    {
        ProfessionalProfileDTO profile = CreateProfile();
        _mediatorMock
            .Setup(M => M.Send(It.IsAny<GetMyProfessionalProfileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        IActionResult result = await _controller.GetMyProfessionalProfileAsync();

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(profile, ok.Value);
    }

    [Fact]
    public async Task GetMyProfessionalProfileAsync_NoEmail_Returns401Async()
    {
        _currentUserServiceMock.Setup(C => C.Email).Returns((string?)null);

        IActionResult result = await _controller.GetMyProfessionalProfileAsync();

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task GetMyProfessionalProfileAsync_UserNotSynced_Returns404Async()
    {
        _mediatorMock
            .Setup(M => M.Send(It.IsAny<GetMyProfessionalProfileQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("User not found."));

        IActionResult result = await _controller.GetMyProfessionalProfileAsync();

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateMyProfessionalProfileAsync_ValidRequest_Returns200WithSavedProfileAsync()
    {
        ProfessionalProfileDTO saved = CreateProfile();
        _mediatorMock
            .Setup(M => M.Send(It.IsAny<UpdateMyProfessionalProfileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(saved);

        IActionResult result = await _controller.UpdateMyProfessionalProfileAsync(new UpdateProfessionalProfileRequest());

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(saved, ok.Value);
    }

    [Fact]
    public async Task UpdateMyProfessionalProfileAsync_NoEmail_Returns401Async()
    {
        _currentUserServiceMock.Setup(C => C.Email).Returns((string?)null);

        IActionResult result = await _controller.UpdateMyProfessionalProfileAsync(new UpdateProfessionalProfileRequest());

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task UpdateMyProfessionalProfileAsync_UserNotSynced_Returns404Async()
    {
        _mediatorMock
            .Setup(M => M.Send(It.IsAny<UpdateMyProfessionalProfileCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("User not found."));

        IActionResult result = await _controller.UpdateMyProfessionalProfileAsync(new UpdateProfessionalProfileRequest());

        Assert.IsType<NotFoundResult>(result);
    }
}
