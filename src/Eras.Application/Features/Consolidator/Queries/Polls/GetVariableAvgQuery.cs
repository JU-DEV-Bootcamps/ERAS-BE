using Eras.Application.Models.Consolidator;
using Eras.Application.Models.Response.Common;

using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class VariableAvgQuery : IRequest<GetQueryResponse<AvgReportResponseVm>>
{
    public required List<int> PollVariableIds { get; set; }
}
