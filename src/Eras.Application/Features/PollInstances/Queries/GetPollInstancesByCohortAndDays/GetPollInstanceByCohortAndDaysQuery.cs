
using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using MediatR;

namespace Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays
{
    public class GetPollInstanceByCohortAndDaysQuery : IRequest<GetQueryResponse<IEnumerable<PollInstanceDTO>>>
    {
        public int CohortId { get; set; }
        public int Days { get; set; }

        public GetPollInstanceByCohortAndDaysQuery(int CohortId, int Days)
        {
            this.CohortId = CohortId;
            this.Days = Days;
        }
    }
}
