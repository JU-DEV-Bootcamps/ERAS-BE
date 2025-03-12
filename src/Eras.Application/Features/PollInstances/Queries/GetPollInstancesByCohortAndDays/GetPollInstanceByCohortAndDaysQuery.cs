
using Eras.Application.DTOs;
using Eras.Application.Models;
using MediatR;

namespace Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays
{
    public class GetPollInstanceByCohortAndDaysQuery : IRequest<GetQueryResponse<IEnumerable<PollInstanceDTO>>>
    {
        public int CohortId { get; set; }
        public int Days { get; set; }

        public GetPollInstanceByCohortAndDaysQuery(int cohortId, int days)
        {
            CohortId = cohortId;
            Days = days;
        }
    }
}
