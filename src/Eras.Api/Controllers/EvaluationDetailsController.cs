using Eras.Application.Features.EvaluationDetails.Queries.GetEvaluationDetailsByFilters;
using Eras.Application.Features.EvaluationDetails.Queries.GetStudentsByFilters;
using Eras.Application.Features.Evaluations.Commands.DeleteEvaluation;
using Eras.Application.Features.Evaluations.Queries.GetByDateRange;
using Eras.Application.Models.Response;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

[Route("api/v1/evaluations")]
[ApiController]
public class EvaluationDetailsController(IMediator Mediator, ILogger<EvaluationsController> Logger) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<EvaluationsController> _logger = Logger;

    [HttpGet("EvaluationDetailsByFilter")]
    public async Task<IActionResult> EvaluationDetailsByFilterAsync([FromQuery] int? PollId, [FromQuery] List<int>? ComponentIds, [FromQuery] List<int>? CohortIds, [FromQuery] List<int>? VariableIds)
    {
        _logger.LogInformation("Retrieving evaluation details with filters {PollId}, Components ({ComponentIds}), Cohorts ({CohortIds}), Variables ({VariableIds})", PollId, ComponentIds, CohortIds, VariableIds);
        var query = new GetEvaluationDetailsByFiltersQuery() { 
            PollId = PollId,
            ComponentIds = ComponentIds,
            CohortIds = CohortIds,
            VariableIds = VariableIds
        };
        var response = await _mediator.Send(query);

        _logger.LogInformation("Successfully retrieved Evaluation Details {response}", response);
        return response == null ? NotFound(response) : Ok(response);
    }

    [HttpGet("StudentsByFilters")]
    public async Task<IActionResult> StudentsByFilterAsync([FromQuery] int? PollId, [FromQuery] List<int>? ComponentIds, [FromQuery] List<int>? CohortIds, [FromQuery] List<int>? VariableIds)
    {
        _logger.LogInformation("Retrieving students with filters {PollId}, Components ({ComponentIds}), Cohorts ({CohortIds}), Variables ({VariableIds})", PollId, ComponentIds, CohortIds, VariableIds);
        var query = new GetStudentsByFiltersQuery()
        {
            PollId = PollId,
            ComponentIds = ComponentIds,
            CohortIds = CohortIds,
            VariableIds = VariableIds
        };
        var response = await _mediator.Send(query);

        _logger.LogInformation("Successfully retrieved Students List {response}", response);
        return response == null ? NotFound(response) : Ok(response);
    }
}
