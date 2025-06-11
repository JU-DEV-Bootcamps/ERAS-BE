
using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;

using MediatR;

namespace Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays
{
    public class GetPollInstanceByCohortAndDaysQuery : IRequest<GetQueryResponse<PagedResult<PollInstanceDTO>>>
    {
        public int[] CohortId { get; set; }
        public int Days { get; set; }
        public Pagination Pagination { get; set; }

        public GetPollInstanceByCohortAndDaysQuery(
                Pagination Pagination,
                int[] CohortId,
                int Days
        )
        {
            this.CohortId = CohortId;
            this.Days = Days;
            this.Pagination = Pagination;
        }
    }
}
