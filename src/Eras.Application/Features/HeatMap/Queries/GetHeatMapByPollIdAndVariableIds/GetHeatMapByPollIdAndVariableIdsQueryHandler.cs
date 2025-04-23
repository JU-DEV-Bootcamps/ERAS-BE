using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.HeatMap;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapByPollIdAndVariableIds
{
    public class GetHeatMapByPollIdAndVariableIdsQueryHandler
        : IRequestHandler<GetHeatMapByPollIdAndVariableIdsQuery, List<HeatMapBaseData>>
    {
        private readonly IHeatMapRepository _heatMapRepository;
        private readonly ILogger<GetHeatMapByPollIdAndVariableIdsQueryHandler> _logger;

        public GetHeatMapByPollIdAndVariableIdsQueryHandler(
            IHeatMapRepository HeatmapRepository,
            ILogger<GetHeatMapByPollIdAndVariableIdsQueryHandler> Logger
        )
        {
            _logger = Logger;
            _heatMapRepository = HeatmapRepository;
        }

        public async Task<List<HeatMapBaseData>> Handle(
            GetHeatMapByPollIdAndVariableIdsQuery Request,
            CancellationToken CancellationToken
        )
        {
            var heatmap = await _heatMapRepository.GetHeatMapByPollUuidAndVariableIds(
                Request.pollUuid,
                Request.variableIds
            );
            return heatmap;
        }
    }
}
