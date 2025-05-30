using Eras.Application.Models.Response.Common;

using MediatR;

namespace Eras.Application.Features.Consolidator.Queries;

public class GetCountSummaryQuery : IRequest<GetQueryResponse<Dictionary<string, int>>>
{
}
