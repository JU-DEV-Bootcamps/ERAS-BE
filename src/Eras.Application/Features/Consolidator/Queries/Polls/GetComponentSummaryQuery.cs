using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class GetComponentSummaryQuery : IRequest<GetQueryResponse<List<Answer>>>
{
    public required Guid PollUuid { get; set; }
}
