
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
        public bool LastVersion { get; set; }
        public string PollUuid { get; set; } = string.Empty;
        public GetPollInstanceByCohortAndDaysQuery(
                Pagination Pagination,
                int[] CohortId,
                int Days,
                bool LastVersion,
                string PollUuid
        )
        {
            this.CohortId = CohortId;
            this.Days = Days;
            this.LastVersion = LastVersion;
            this.PollUuid = PollUuid;
        }
    }
}