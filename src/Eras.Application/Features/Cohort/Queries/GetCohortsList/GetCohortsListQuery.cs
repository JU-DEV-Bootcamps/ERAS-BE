using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Cohorts.Queries
{
    public class GetCohortsListQuery : IRequest<GetQueryResponse<List<Cohort>>>
    {
        public string PollUuid { get; set; } = string.Empty;
    }
}
