using Eras.Application.DTOs.Views;
using Eras.Application.Utils;

using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class GetPollTopQuery : IRequest<PagedResult<ErasCalculationsByPollDTO>?>
{
    public required Guid PollUuid { get; set; }
    public required Pagination Pagination { get; set; }
    public required int VariableIds { get; set; }
}
