using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Features.ErasUsers;
using Eras.Application.Features.ErasUsers.Models;
using Eras.Domain.Entities.UserManagement;
using Eras.Error.Bussiness;
using Eras.Infrastructure.Authorization;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

[Route("api/v1/users")]
[ApiController]
[Authorize]
public class UsersController(
    IMediator Mediator,
    ICurrentUserService CurrentUserService,
    KeycloakRoleNames RoleNames
) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ICurrentUserService _currentUserService = CurrentUserService;
    private readonly KeycloakRoleNames _roleNames = RoleNames;

    /// <summary>
    /// Upserts the local ERAS user profile for the authenticated identity, from the
    /// role and identity claims carried by its Keycloak-issued access token. Meant to
    /// be called once per session by clients that authenticate directly against
    /// Keycloak (bypassing the backend's own /auth/login), so their profile still gets
    /// registered/kept in sync with `eras_users`.
    /// </summary>
    [HttpPost("sync")]
    public async Task<IActionResult> SyncAsync()
    {
        if (_currentUserService.Email is not { } email)
            return Unauthorized();

        var command = new SyncErasUserCommand(
            _currentUserService.Sub,
            email,
            _currentUserService.FirstName ?? string.Empty,
            _currentUserService.LastName ?? string.Empty,
            ErasRole.Resolve(_currentUserService.Roles, _roleNames)
        );

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Returns the current user's ERAS role. No role restriction beyond being
    /// authenticated: even a Guest (no ERAS role assigned yet) should be able to
    /// check their own role.
    /// </summary>
    [HttpGet("me/role")]
    public IActionResult GetMyRole()
    {
        var role = ErasRole.Resolve(_currentUserService.Roles, _roleNames);
        return Ok(new { role });
    }

    /// <summary>
    /// Lists real ERAS users, optionally filtered by role (e.g. to populate an "assigned
    /// professional" picker with actual Keycloak-synced accounts, or — with no role — to
    /// resolve any user's sub back to a display name when rendering assessments/interventions).
    /// </summary>
    [HttpGet]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    public async Task<IActionResult> GetByRole([FromQuery] string? role)
    {
        var result = await _mediator.Send(new GetErasUsersByRoleQuery(role));
        return Ok(result);
    }

    /// <summary>
    /// Returns the current user's own profile (name, email, role, the editable
    /// employee fields and the read-only counters of their active assessments and
    /// interventions). No role restriction beyond being authenticated.
    /// </summary>
    [HttpGet("me/profile")]
    public async Task<IActionResult> GetMyProfileAsync()
    {
        if (_currentUserService.Email is not { } email)
            return Unauthorized();

        try
        {
            var result = await _mediator.Send(new GetMyProfileQuery(_currentUserService.Sub, email));
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Updates the authenticated caller's own editable profile fields (Employee ID,
    /// department, phone, position, about/bio). Only the same user can update these
    /// attributes; first name, last name, email, and role stay exclusively
    /// Keycloak-sync-owned and are never editable here.
    /// </summary>
    [HttpPut("me/profile")]
    [Authorize(Policy = ErasPolicies.AnyErasRole)]
    public async Task<IActionResult> UpdateUserProfileAsync([FromBody] UpdateUserProfileRequest request)
    {
        if (_currentUserService.Email is not { } email)
            return Unauthorized();

        try
        {
            var command = new UpdateUserProfileCommand(
                _currentUserService.Sub,
                email,
                request.EmployeeId,
                request.Department,
                request.Phone,
                request.Position,
                request.About
            );

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
