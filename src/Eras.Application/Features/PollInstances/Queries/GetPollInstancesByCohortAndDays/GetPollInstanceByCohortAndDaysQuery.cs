
using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;

using MediatR;

namespace Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays
{
    public class GetPollInstanceByCohortAndDaysQuery(
            Pagination Pagination,
            int[] CohortId,
            int Days,
            bool LastVersion,
            string PollUuid
        ) : IRequest<GetQueryResponse<PagedResult<PollInstanceDTO>>>
    {
        public int[] CohortId { get; set; } = CohortId;
        public int Days { get; set; } = Days;

        public Pagination Pagination { get; set; } = Pagination;
        public bool LastVersion { get; set; } = LastVersion;
        public string PollUuid { get; set; } = PollUuid;
    }
}