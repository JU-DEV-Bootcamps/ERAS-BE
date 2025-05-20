using Eras.Application.DTOs.Views;

using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class GetPollTopQuery : IRequest<List<ErasCalculationsByPollDTO>?>
{
    public required Guid PollUuid { get; set; }
    public required int Take { get; set; } = 5;
    public required int VariableIds { get; set; }
}
