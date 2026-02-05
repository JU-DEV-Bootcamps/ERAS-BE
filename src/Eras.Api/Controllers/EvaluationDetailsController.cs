using System.ComponentModel.DataAnnotations;

using Eras.Application.Features.EvaluationDetails.Queries.GetStudentsByEvaluationId;
using Eras.Application.Features.EvaluationDetails.Queries.GetStudentsByFilters;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Eras.Api.Controllers;

[Route("api/v1/evaluations")]
[ApiController]
public class EvaluationDetailsController(IMediator Mediator, ILogger<EvaluationsController> Logger) : ControllerBase
{
    private readonly IMediator _mediator = Mediator;
    private readonly ILogger<EvaluationsController> _logger = Logger;

    [HttpGet("StudentsByFilters")]
    public async Task<IActionResult> StudentsByFilterAsync([FromQuery, Required] string PollUuid, [FromQuery, Required, MinLength(1)] List<string> ComponentNames, [FromQuery, Required, MinLength(1)] List<int> CohortIds, [FromQuery, Required, MinLength(1)] List<int>? VariableIds, [FromQuery] List<decimal>? RiskLevels)
    {
        _logger.LogInformation("Retrieving students with filters {PollId}, Components ({ComponentIds}), Cohorts ({CohortIds}), Variables ({VariableIds})", PollUuid, ComponentNames, CohortIds, VariableIds);
        var query = new GetStudentsByFiltersQuery()
        {
            PollUuid = PollUuid,
            ComponentNames = ComponentNames,
            CohortIds = CohortIds,
            VariableIds = VariableIds,
            RiskLevels = RiskLevels
        };
        var response = await _mediator.Send(query);

        _logger.LogInformation("Successfully retrieved Students List {response}", response);
        return response == null ? NotFound(response) : Ok(response);
    }

    [HttpGet("StudentsByEvaluationId")]
    public async Task<IActionResult> StudentsByEvaluationIdAsync([FromQuery, Required] int EvaluationId, [FromQuery, Required, MinLength(1)] List<string> ComponentNames, [FromQuery, Required, MinLength(1)] List<int> CohortIds, [FromQuery, Required, MinLength(1)] List<int>? VariableIds, [FromQuery] List<decimal>? RiskLevels)
    {
        _logger.LogInformation("Retrieving students with filters {EvaluationId}, Components ({ComponentIds}), Cohorts ({CohortIds}), Variables ({VariableIds})", EvaluationId, ComponentNames, CohortIds, VariableIds);
        var query = new GetStudentsByEvaluationIdQuery()
        {
            EvaluationId = EvaluationId,
            ComponentNames = ComponentNames,
            CohortIds = CohortIds,
            VariableIds = VariableIds,
            RiskLevels = RiskLevels
        };
        var response = await _mediator.Send(query);

        _logger.LogInformation("Successfully retrieved Students List {response}", response);
        return response == null ? NotFound(response) : Ok(response);
    }
}
