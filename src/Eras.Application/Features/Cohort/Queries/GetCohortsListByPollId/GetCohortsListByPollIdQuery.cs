using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Cohorts.Queries
{
    public class GetCohortsListByPollIdQuery : IRequest<GetQueryResponse<List<Cohort>>>
    {
        public int? PollId { get; set; }
    }
}
