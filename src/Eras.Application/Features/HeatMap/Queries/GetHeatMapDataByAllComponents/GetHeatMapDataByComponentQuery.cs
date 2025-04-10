﻿using Eras.Application.Models;
using Eras.Application.Models.HeatMap;

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
