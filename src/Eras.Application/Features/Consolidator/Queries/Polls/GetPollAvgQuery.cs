using Eras.Application.Models;
using Eras.Application.Models.Consolidator;

using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class PollAvgQuery : IRequest<GetQueryResponse<AvgReportResponseVm>>
{
    public required Guid PollUuid { get; set; }
}
