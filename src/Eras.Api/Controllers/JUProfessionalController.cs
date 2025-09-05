using Eras.Application.DTOs;
using Eras.Application.Features.Professionals.Commands.CreateProfessional;
using Eras.Application.Features.Professionals.Queries.GetProfessionals;
using Eras.Application.Utils;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/professionals")]
[ExcludeFromCodeCoverage]
public class ProfessionalsController(IMediator Mediator, ILogger<ProfessionalsController> Logger) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<ProfessionalsController> _logger = Logger;
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProfessionalsAsync([FromQuery] Pagination Query)
    {
        return Ok(await _mediator.Send(new GetProfessionalsQuery() { Query = Query }));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProfessionalAsync([FromBody] JUProfessionalDTO Professional)
    {
        if (Professional == null)
        {
            _logger.LogError("Professional is null");
            return BadRequest("Professional cannot be null");
        }
        CreateProfessionalCommand createProfessionalCommand = new CreateProfessionalCommand()
        {
            Professional = Professional
        };
        var result = await _mediator.Send(createProfessionalCommand);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result.Message);
    }
}