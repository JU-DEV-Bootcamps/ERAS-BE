using Eras.Application.Features.Dashboard.Queries.GetDashboardKpis;
using Eras.Infrastructure.Authorization;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = ErasPolicies.AnyErasRole)]
public class DashboardController(IMediator Mediator) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;

    [HttpGet("kpis")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetKpis()
    {
        var result = await _mediator.Send(new GetDashboardKpisQuery());

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}