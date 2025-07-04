using Eras.Application.DTOs;
using Eras.Application.Features.Configurations.Command.CreateConfiguration;
using Eras.Application.Features.Configurations.Queries.GetAllConfigurations;
using Eras.Application.Features.ServiceProviders.Command;
using Eras.Application.Features.ServiceProviders.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/service-providers")]
[ExcludeFromCodeCoverage]
public class ServiceProvidersController(IMediator Mediator, ILogger<ServiceProvidersController> Logger) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<ServiceProvidersController> _logger = Logger;
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetServiceProvidersAsync()
    {
        return Ok(await _mediator.Send(new GetAllServiceProvidersQuery()));
    }

    [HttpPost]
    public async Task<IActionResult> CreateServiceProviderAsync([FromBody] ServiceProvidersDTO ServiceProvider)
    {
        if (ServiceProvider == null)
        {
            _logger.LogError("Service Provider is null");
            return BadRequest("Service Provider cannot be null");
        }
        CreateServiceProviderCommand createServiceProviderCommand = new CreateServiceProviderCommand()
        {
            ServiceProviders = ServiceProvider
        };
        var result = await _mediator.Send(createServiceProviderCommand);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result.Message);
    }
}
