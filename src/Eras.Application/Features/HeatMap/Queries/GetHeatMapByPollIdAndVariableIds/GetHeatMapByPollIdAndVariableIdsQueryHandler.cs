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
            IHeatMapRepository heatmapRepository,
            ILogger<GetHeatMapByPollIdAndVariableIdsQueryHandler> logger
        )
        {
            _logger = logger;
            _heatMapRepository = heatmapRepository;
        }

        public async Task<List<HeatMapBaseData>> Handle(
            GetHeatMapByPollIdAndVariableIdsQuery request,
            CancellationToken cancellationToken
        )
        {
            var heatmap = await _heatMapRepository.GetHeatMapByPollUuidAndVariableIds(
                request.pollUuid,
                request.variableIds
            );
            return heatmap;
        }
    }
}
