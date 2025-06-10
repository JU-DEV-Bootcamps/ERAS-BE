using Eras.Application.Models.Consolidator;
using Eras.Application.Models.Response.Common;

using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class GetRiskCountQuery : IRequest<GetQueryResponse<RiskCountResponseVm>>
{
    public required Guid PollUuid { get; set; }
}
