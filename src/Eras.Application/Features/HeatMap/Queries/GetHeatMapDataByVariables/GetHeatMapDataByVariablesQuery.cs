using Eras.Application.Models.HeatMap;
using Eras.Application.Models;
using MediatR;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByVariables
{
    public class GetHeatMapDataByVariablesQuery : IRequest<GetQueryResponse<HeatMapByVariablesResponseVm>>
    {
        public string PollInstanceUUID { get; set; }
        public string Component { get; set; }

        public GetHeatMapDataByVariablesQuery(string component, string pollInstanceUUID)
        {
            this.PollInstanceUUID = pollInstanceUUID;
            this.Component = component;
        }
    }
}
