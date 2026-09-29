using System.Diagnostics.CodeAnalysis;

using Eras.Application.DTOs;
using Eras.Application.Features.FeatureFlags;
using Eras.Error.Bussiness;
using Eras.Infrastructure.Authorization;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

/// <summary>
/// Reads require only an ERAS role (the FE checks flags for any logged-in user);
/// mutations are narrowed to Admin via a stricter policy on top of the class one
/// ("Manage application version displayed to users").
/// </summary>
[ApiController]
[Route("api/v1/feature-flags")]
[Authorize(Policy = ErasPolicies.AnyErasRole)]
public class FeatureFlagController(IMediator Mediator, ILogger<FeatureFlagController> Logger): ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<FeatureFlagController> _logger = Logger;

    [HttpGet("{Name}")]
    [ProducesResponseType(typeof(FeatureFlagDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FeatureFlagDTO), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FeatureFlagDTO?>> GetFeatureFlagByNameAsync(string Name)
    {
        try
        {
            FeatureFlagDTO? response = await _mediator.Send(new GetFeatureFlagByNameQuery(Name));
            return Ok(response);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<FeatureFlagDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<FeatureFlagDTO>>> GetAllAsync()
    {
        IReadOnlyCollection<FeatureFlagDTO> response = await _mediator.Send(new GetAllFeatureFlagsQuery());
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Policy = ErasPolicies.AdminOnly)]
    [ProducesResponseType(typeof(FeatureFlagDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FeatureFlagDTO>> CreateFeatureFlagAsync([FromBody] FeatureFlagDTO DTO)
    {
        try
        {
           FeatureFlagDTO response = await _mediator.Send(new CreateFeatureFlagCommand(DTO));
            return Created(string.Empty, response); 
        }
        catch(InvalidOperationException Ex)
        {
            return Conflict(new { Status=409, Ex.Message });
        }
    }

    [HttpPut("{Id:int}")]
    [Authorize(Policy = ErasPolicies.AdminOnly)]
    [ProducesResponseType(typeof(FeatureFlagDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FeatureFlagDTO>> UpdatedFeatureFlagAsync(int Id, [FromBody] FeatureFlagDTO DTO)
    {
        DTO.Id = Id;
        FeatureFlagDTO response = await _mediator.Send(new UpdateFeatureFlagCommand(DTO));
        return Ok(response);
    }

    [HttpDelete("{Id:int}")]
    [Authorize(Policy = ErasPolicies.AdminOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteFeatureFlagAsync(int Id)
    {
        try
        {
            await _mediator.Send(new DeleteFeatureFlagCommand(Id));
            _logger.LogInformation($"Feature Flag with ID ${Id} has been deleted.");
            return NoContent();
        }
        catch(KeyNotFoundException Exception)
        {
            _logger.LogError(Exception.Message);
            return NotFound();
        }
    }
    
}