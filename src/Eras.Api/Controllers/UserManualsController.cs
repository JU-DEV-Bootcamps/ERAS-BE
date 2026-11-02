using Eras.Application.DTOs.UserManuals;
using Eras.Application.Features.UserManuals;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

/// <summary>
/// Read-only user manuals. Open to any signed-in user, Guest included (no role policy), and scoped to
/// the manuals declared in the repository (ADR-0004).
/// </summary>
[ApiController]
[Route("api/v1/manuals")]
[Authorize]
public class UserManualsController(IMediator Mediator) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<UserManualDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyCollection<UserManualDto>>> GetUserManualsAsync(
        CancellationToken CancellationToken)
    {
        IReadOnlyCollection<UserManualDto> manuals = await _mediator.Send(new GetUserManualsQuery(), CancellationToken);
        return Ok(manuals);
    }

    [HttpGet("{Id}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadUserManualAsync([FromRoute] string Id, CancellationToken CancellationToken)
    {
        UserManualFileDto file = await _mediator.Send(new DownloadUserManualQuery(Id), CancellationToken);
        return File(file.Content, file.ContentType, file.FileName, enableRangeProcessing: false);
    }
}
