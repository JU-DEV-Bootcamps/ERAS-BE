using Eras.Application.Models.Response.Common;

using MediatR;

namespace Eras.Application.Features.Cohorts.Queries
{
    public class GetCohortsListQuery : IRequest<GetQueryResponse<List<Domain.Entities.Cohort>>>
    {
        public string PollUuid { get; set; } = string.Empty;
    }
}
