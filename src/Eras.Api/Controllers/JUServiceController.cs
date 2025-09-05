using Eras.Application.DTOs;
using Eras.Application.Features.JUServices.Commands.CreateJUService;
using Eras.Application.Features.JUServices.Queries.GetJUServices;
using Eras.Application.Utils;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/ju_services")]
[ExcludeFromCodeCoverage]
public class JUServiceController(IMediator Mediator, ILogger<JUServiceController> Logger) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<JUServiceController> _logger = Logger;
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetServicesAsync([FromQuery] Pagination Query)
    {
        return Ok(await _mediator.Send(new GetJUServicesQuery() { Query = Query }));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateServiceAsync([FromBody] JUServiceDTO Service)
    {
        if (Service == null)
        {
            _logger.LogError("Service is null");
            return BadRequest("Service cannot be null");
        }
        CreateJUServiceCommand createJUServiceCommand = new CreateJUServiceCommand()
        {
            Service = Service
        };
        var result = await _mediator.Send(createJUServiceCommand);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result.Message);
    }
}