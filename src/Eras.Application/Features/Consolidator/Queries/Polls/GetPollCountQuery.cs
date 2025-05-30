using Eras.Application.Models.Consolidator;
using Eras.Application.Models.Response.Common;

using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class PollCountQuery : IRequest<GetQueryResponse<CountReportResponseVm>>
{
    public required string PollUuid { get; set; }
    public required List<int> CohortIds { get; set; }
    public required List<int> VariableIds { get; set; }
}
