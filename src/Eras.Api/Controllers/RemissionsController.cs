using System.Diagnostics.CodeAnalysis;

using Eras.Application.Features.Remmisions.Queries.GetRemissions;
using Eras.Application.Models.Response.Controllers.RemissionsController;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/remissions")]
[ExcludeFromCodeCoverage]

public class RemissionsController(IMediator Mediator, ILogger<RemissionsController> Logger) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<RemissionsController> _logger = Logger;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRemissionsAsync([FromQuery] Pagination Query)
    {
        List<JURemission> Result = await _mediator.Send(new GetRemissionsQuery());
        return Ok(Result);
    }
    
    /*[HttpGet("{Id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRemissionByIdAsync(
        [FromRoute] int Id
    ) => Ok(await _mediator.Send(new GetRemissionByIdQuery(Id)));*/

}