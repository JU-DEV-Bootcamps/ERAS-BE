using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Models.HeatMap;
using Eras.Application.Models;
using MediatR;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapSummary
{
    public class GetHeatMapSummaryQuery : IRequest<GetQueryResponse<IEnumerable<HeatMapSummaryResponseVm>>>
    {
        public string PollInstanceUUID { get; }

        public GetHeatMapSummaryQuery(string pollInstanceUUID) { 
            PollInstanceUUID = pollInstanceUUID;
        }
    }
}
