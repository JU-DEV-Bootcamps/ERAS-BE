
using Eras.Application.Models;
using Eras.Application.Models.HeatMap;

using MediatR;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapSummaryByFilters
{
    public class GetHeatMapSummaryByFiltersQuery : IRequest<GetQueryResponse<HeatMapSummaryResponseVm>>
    {
        public int CohortId { get; }
        public int Days { get; }

        public GetHeatMapSummaryByFiltersQuery(int cohortId, int days)
        {
            CohortId = cohortId;
            Days = days;
        }
    }
}
