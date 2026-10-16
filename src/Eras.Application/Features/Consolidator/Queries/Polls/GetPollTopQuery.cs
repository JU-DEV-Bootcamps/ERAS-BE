using Eras.Application.DTOs.Views;
using Eras.Application.Utils;

using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class GetPollTopQuery : IRequest<PagedResult<ErasCalculationsByPollDTO>?>
{
    public required Guid PollUuid { get; set; }
    public required Pagination Pagination { get; set; }
    public required List<int> VariableIds { get; set; }
    /// <summary>When set, the report covers every poll of that evaluation instead of the single poll uuid.</summary>
    public int? EvaluationId { get; set; }
}
