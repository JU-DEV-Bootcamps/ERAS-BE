using MediatR;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Response.HeatMap;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapSummary
{
    public class GetHeatMapSummaryQuery : IRequest<GetQueryResponse<HeatMapSummaryResponseVm>>
    {
        public string PollInstanceUUID { get; }

        public GetHeatMapSummaryQuery(string PollInstanceUUID) { 
            this.PollInstanceUUID = PollInstanceUUID;
        }
    }
}
