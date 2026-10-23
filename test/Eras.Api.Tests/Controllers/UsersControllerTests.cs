using Eras.Api.Controllers;
using Eras.Application.Contracts.Infrastructure;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Features.ErasUsers;
using Eras.Application.Features.ErasUsers.Models;
using Eras.Domain.Common;
using Eras.Domain.Entities.UserManagement;
using Eras.Error.Bussiness;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace Eras.Api.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _controller = new UsersController(_mediatorMock.Object, _currentUserServiceMock.Object, new KeycloakRoleNames());
    }

    private static ErasUserDTO CreateProfile() => new()
    {
        Id = 1,
        Sub = "sub-123",
        Email = "user@test.com",
        FirstName = "First",
        LastName = "Last",
        Role = ErasRole.Officer.Label,
        Audit = new AuditInfo { CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
    };

    [Fact]
    public async Task GetMyProfileAsync_ReturnsOk_WhenProfileExistsAsync()
    {
        _currentUserServiceMock.Setup(C => C.Email).Returns("user@test.com");
        _currentUserServiceMock.Setup(C => C.Sub).Returns("sub-123");

        var profile = MyProfileDTO.From(CreateProfile(), ActiveAssessments: 4, ActiveInterventions: 2);
        _mediatorMock
            .Setup(M => M.Send(It.IsAny<GetMyProfileQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        var result = await _controller.GetMyProfileAsync();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(profile, ok.Value);
    }

    [Fact]
    public async Task GetMyProfileAsync_ReturnsUnauthorized_WhenCurrentUserHasNoEmailAsync()
    {
        _currentUserServiceMock.Setup(C => C.Email).Returns((string?)null);

        var result = await _controller.GetMyProfileAsync();

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task GetMyProfileAsync_ReturnsNotFound_WhenUserNotYetSyncedAsync()
    {
        _currentUserServiceMock.Setup(C => C.Email).Returns("user@test.com");
        _mediatorMock
            .Setup(M => M.Send(It.IsAny<GetMyProfileQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("not found"));

        var result = await _controller.GetMyProfileAsync();

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateUserProfileAsync_ReturnsOk_WhenSuccessfulAsync()
    {
        _currentUserServiceMock.Setup(C => C.Email).Returns("user@test.com");
        _currentUserServiceMock.Setup(C => C.Sub).Returns("sub-123");

        var profile = CreateProfile();
        _mediatorMock
            .Setup(M => M.Send(It.IsAny<UpdateUserProfileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        var request = new UpdateUserProfileRequest { EmployeeId = "E-1", Department = "IT" };
        var result = await _controller.UpdateUserProfileAsync(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(profile, ok.Value);
    }

    [Fact]
    public async Task UpdateUserProfileAsync_ReturnsUnauthorized_WhenCurrentUserHasNoEmailAsync()
    {
        _currentUserServiceMock.Setup(C => C.Email).Returns((string?)null);

        var request = new UpdateUserProfileRequest { EmployeeId = "E-1" };
        var result = await _controller.UpdateUserProfileAsync(request);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task UpdateUserProfileAsync_ReturnsNotFound_WhenUserDoesNotExistAsync()
    {
        _currentUserServiceMock.Setup(C => C.Email).Returns("user@test.com");
        _mediatorMock
            .Setup(M => M.Send(It.IsAny<UpdateUserProfileCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("not found"));

        var request = new UpdateUserProfileRequest { EmployeeId = "E-1" };
        var result = await _controller.UpdateUserProfileAsync(request);

        Assert.IsType<NotFoundResult>(result);
    }
}
