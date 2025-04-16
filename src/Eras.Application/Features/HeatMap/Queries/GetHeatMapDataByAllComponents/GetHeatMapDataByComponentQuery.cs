using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Response.HeatMap;
using MediatR;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents
{
    public class GetHeatMapDataByAllComponentsQuery : IRequest<GetQueryResponse<IEnumerable<HeatMapByComponentsResponseVm>>>
    {
        public string PollInstanceUUID { get; }

        public GetHeatMapDataByAllComponentsQuery(string pollInstanceUUID)
        {
            PollInstanceUUID = pollInstanceUUID;
        }
    }
}
