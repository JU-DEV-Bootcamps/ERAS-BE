using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.Configurations.Queries.GetConfiguration;
using Eras.Application.Services;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Eras.Api.Controllers;

[ApiController]
[Route("api/v1/cosmic-latte")]
public class CosmicLatteController(IMediator Mediator, ICosmicLatteAPIService CosmicLatteService, IImportJobService ImportJobService) : ControllerBase
{
    private readonly ICosmicLatteAPIService _cosmicLatteService = CosmicLatteService;
    private readonly IMediator _mediator = Mediator;
    private readonly IImportJobService _importJobService = ImportJobService;

    [HttpGet("polls")]
    public async Task<IActionResult> GetPreviewPollsAsync(
        [FromQuery] string EvaluationSetName = "",
        [FromQuery] string StartDate = "",
        [FromQuery] string EndDate = "",
        [FromQuery] int ConfigurationId = 0
    )
    {
        if (EvaluationSetName.Length > 100)
            return BadRequest(new { message = "There was an error during the import: Poll Name exceeds the maximum length of 100 characters." });

        try
        {
            GetConfigurationQuery getConfigurationQuery = new GetConfigurationQuery { ConfigurationId = ConfigurationId };
            var configuration = await _mediator.Send(getConfigurationQuery);
            return Ok(await _cosmicLatteService.GetAllPollsPreview(EvaluationSetName, StartDate, EndDate, configuration.EncryptedKey, configuration.BaseURL));
        }
        catch (ArgumentException ex)
        {
            if (EvaluationSetName.Length == 100 && ex.Message.Contains("Evaluation not found"))
            {
                return BadRequest(new { message = "There was an error during the import: Poll Name exceeds the maximum length of 100 characters." });
            }
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidCastException ex)
        {
            return BadRequest(new { message = "Error deserializing response from Cosmic Latte API", detailed = ex.StackTrace });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("health")]
    public async Task<IActionResult> IsCosmicLatteApiHealthyAsync([FromQuery] int ConfigurationId)
    {
        try
        {
            GetConfigurationQuery getConfigurationQuery = new GetConfigurationQuery { ConfigurationId = ConfigurationId };
            var configuration = await _mediator.Send(getConfigurationQuery);
            return Ok(await _cosmicLatteService.CosmicApiIsHealthy(configuration.EncryptedKey, configuration.BaseURL));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("polls/{EvaluationId}")]
    public async Task<IActionResult> SavePreviewPollsAsync([FromBody] List<PollDTO> PollsInstances, int EvaluationId)
    {
        try
        {
            // Queue the import for background processing and return immediately; the client polls
            // GET imports/{importJobId} for progress instead of waiting for the whole import.
            int importJobId = await _importJobService.QueueImportAsync(PollsInstances, EvaluationId);
            return AcceptedAtAction(nameof(GetImportStatusAsync), new { ImportJobId = importJobId },
                new { importJobId, status = "Queued" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("imports/{ImportJobId}")]
    public async Task<IActionResult> GetImportStatusAsync(int ImportJobId)
    {
        var status = await _importJobService.GetStatusAsync(ImportJobId);
        if (status == null)
        {
            return NotFound(new { message = $"Import job {ImportJobId} not found" });
        }
        return Ok(status);
    }

    [Authorize]
    [HttpGet("imports/{ImportJobId}/items")]
    public async Task<IActionResult> GetImportItemsAsync(int ImportJobId)
    {
        var status = await _importJobService.GetStatusAsync(ImportJobId);
        if (status == null)
        {
            return NotFound(new { message = $"Import job {ImportJobId} not found" });
        }
        return Ok(await _importJobService.GetItemsAsync(ImportJobId));
    }

    [Authorize]
    [HttpPost("imports/{ImportJobId}/retry")]
    public async Task<IActionResult> RetryImportItemsAsync(int ImportJobId, [FromBody] RetryImportItemsRequest Request)
    {
        if (Request?.ItemIds == null || Request.ItemIds.Count == 0)
        {
            return BadRequest(new { message = "No items provided to retry." });
        }
        bool found = await _importJobService.RetryItemsAsync(ImportJobId, Request.ItemIds);
        if (!found)
        {
            return NotFound(new { message = $"Import job {ImportJobId} not found" });
        }
        return Accepted(new { importJobId = ImportJobId, status = "Queued" });
    }

    [HttpGet("polls/names")]
    public async Task<IActionResult> GetPollsNameListAsync([FromQuery] int ConfigurationId)
    {
        try
        {
            GetConfigurationQuery getConfigurationQuery = new GetConfigurationQuery { ConfigurationId = ConfigurationId };
            var configuration = await _mediator.Send(getConfigurationQuery);
            return Ok(await _cosmicLatteService.GetPollsNameList(configuration.BaseURL, configuration.EncryptedKey));
        }
        catch (InvalidCastException ex)
        {
            return BadRequest(new { message = "Error deserializing response from Cosmic Latte API", detailed = ex.StackTrace });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}