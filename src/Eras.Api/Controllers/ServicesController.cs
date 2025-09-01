/*
using Eras.Application.DTOs;
using Eras.Application.Features.JUServices.Commands.CreateJUService;
using Eras.Application.Features.JUServices.Queries.GetJUServices;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/ju_services")]
[ExcludeFromCodeCoverage]
public class JUServicesController(IMediator Mediator, ILogger<JUServicesController> Logger) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<JUServicesController> _logger = Logger;
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetJUServicesAsync()
    {
        return Ok(await _mediator.Send(new GetJUServicesQuery()));
    }

    [HttpPost]
    public async Task<IActionResult> CreateJUServiceAsync([FromBody] JUJUServiceDTO JUService)
    {
        if (JUService == null)
        {
            _logger.LogError("JUService is null");
            return BadRequest("JUService cannot be null");
        }
        CreateJUServiceCommand createJUServiceCommand = new CreateJUServiceCommand()
        {
            JUService = JUService
        };
        var result = await _mediator.Send(createJUServiceCommand);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result.Message);
    }
}
*/