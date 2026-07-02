using Eras.Application.Features.Users.Commands.AssignUserRole;
using Eras.Application.Features.Users.Queries.GetCurrentUserRole;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var result = await _mediator.Send(new GetCurrentUserRoleQuery());
        return Ok(result);
    }

    [HttpPost("roles")]
    [Authorize(Policy = "RequireErasAdmin")]
    public async Task<IActionResult> AssignRole([FromBody] AssignUserRoleCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }
}

