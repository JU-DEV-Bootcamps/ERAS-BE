using Eras.Application.Models;
using Eras.Application.Models.Consolidator;

using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class PollAvgQuery: IRequest<GetQueryResponse<AvgConsolidatorResponseVm>>
{
    public required Guid PollUuid { get; set; }
}
