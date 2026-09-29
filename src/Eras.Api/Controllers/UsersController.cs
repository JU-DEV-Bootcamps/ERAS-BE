using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Features.ErasUsers;
using Eras.Domain.Entities.UserManagement;
using Eras.Infrastructure.Authorization;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

[Route("api/v1/users")]
[ApiController]
[Authorize]
public class UsersController(IMediator Mediator, ICurrentUserService CurrentUserService) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ICurrentUserService _currentUserService = CurrentUserService;

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
            ErasRole.Resolve(_currentUserService.Roles)
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
        var role = ErasRole.Resolve(_currentUserService.Roles);
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
}
