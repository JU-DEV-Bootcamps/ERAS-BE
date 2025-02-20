using Eras.Application.Features.HeatMap.Queries.GetHeatMapDetailsByComponent;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class HeatMapController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<HeatMapController> _logger;

    public HeatMapController(IMediator mediator, ILogger<HeatMapController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("heatmap-details")]
    public async Task<IActionResult> GetStudentHeatMapDetailsByComponent(
        [FromQuery] string component
    )
    {
        var result = await _mediator.Send(new GetHeatMapDetailsByComponentQuery(component));
        return Ok(result);
    }
}
