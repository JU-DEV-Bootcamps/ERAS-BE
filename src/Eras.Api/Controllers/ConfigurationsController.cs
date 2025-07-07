using Eras.Application.DTOs;
using Eras.Application.Features.Configurations.Command.CreateConfiguration;
using Eras.Application.Features.Configurations.Command.DeleteConfiguration;
using Eras.Application.Features.Configurations.Command.EditConfiguration;
using Eras.Application.Features.Configurations.Queries.GetAllConfigurations;
using Eras.Application.Features.Configurations.Queries.GetUserConfigurations;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/configurations")]
[ExcludeFromCodeCoverage]
public class ConfigurationsController(IMediator Mediator, ILogger<ConfigurationsController> Logger) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<ConfigurationsController> _logger = Logger;
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetConfigurationsAsync()
    {
        return Ok(await _mediator.Send(new GetAllConfigurationsQuery()));
    }

    [HttpGet("{UserId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserConfigurationsAsync([FromRoute] string UserId)
    {
        return Ok(await _mediator.Send(new GetUserConfigurationsQuery() { UserId = UserId}));
    }

    [HttpPost]
    public async Task<IActionResult> CreateConfigurationAsync([FromBody] ConfigurationsDTO Configuration)
    {
        if (Configuration == null)
        {
            _logger.LogError("Configuration is null");
            return BadRequest("Configuration cannot be null");
        }
        CreateConfigurationCommand createConfigurationCommand = new CreateConfigurationCommand()
        {
            Configurations = Configuration
        };
        var result = await _mediator.Send(createConfigurationCommand);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result.Message);
    }

    [HttpPut]
    public async Task<IActionResult> EditConfigurationAsync([FromBody] ConfigurationsDTO ConfigurationsDTO)
    {
        if (ConfigurationsDTO == null)
        {
            _logger.LogError("Configuration is null");
            return BadRequest("Configuration cannot be null");
        }
        EditConfigurationCommand EditConfigurationCommand = new EditConfigurationCommand()
        {
            ConfigurationDTO = ConfigurationsDTO
        };
        var result = await _mediator.Send(EditConfigurationCommand);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result.Message);
    }

    [HttpDelete("{ConfigurationId}")]
    public async Task<IActionResult> DeleteConfigurationAsync([FromRoute] int ConfigurationId)
    {
        if (ConfigurationId <= 0)
        {
            _logger.LogError("ConfigurationId is invalid");
            return BadRequest("ConfigurationId must be greater than 0");
        }
        DeleteConfigurationCommand deleteConfigurationCommand = new DeleteConfigurationCommand()
        {
            ConfigurationId = ConfigurationId
        };
        var result = await _mediator.Send(deleteConfigurationCommand);
        if (result.Success)
        {
            return Ok(result);
        }
        return BadRequest(result.Message);
    }

}
