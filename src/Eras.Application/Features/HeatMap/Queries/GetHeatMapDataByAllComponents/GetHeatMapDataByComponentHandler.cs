using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.HeatMap;
using Microsoft.Extensions.Logging;
using MediatR;
using Eras.Application.Exceptions;
using Eras.Application.Models;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents
{
    public class GetHeatMapDataByAllComponentsHandler : IRequestHandler<GetHeatMapDataByAllComponentsQuery, GetQueryResponse<IEnumerable<HeatMapByComponentsResponseVm>>>
    {
        private readonly IHeatMapRepository _heatMapRepository;
        private readonly ILogger<GetHeatMapDataByAllComponentsHandler> _logger;

        public GetHeatMapDataByAllComponentsHandler(IHeatMapRepository heatMapRepository, ILogger<GetHeatMapDataByAllComponentsHandler> logger)
        {
            _heatMapRepository = heatMapRepository;
            _logger = logger;
        }
        public async Task<GetQueryResponse<IEnumerable<HeatMapByComponentsResponseVm>>> Handle(GetHeatMapDataByAllComponentsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.PollInstanceUUID))
            {
                throw new NotFoundException($"Poll instance ID cannot be null or empty");
            }
            try
            {
                var answersByComponents = await _heatMapRepository.GetHeatMapDataByComponentsAsync(request.PollInstanceUUID);
                if (answersByComponents == null || !answersByComponents.Any())
                    throw new NotFoundException($"No data found for poll instance ID: {request.PollInstanceUUID}");


                var bodyData = answersByComponents
            .GroupBy(q => q.ComponentName)
            .Select(g => HeatMapMapper.MaptToVmResponse(g, g.Key))
            .ToList();

                return new GetQueryResponse<IEnumerable<HeatMapByComponentsResponseVm>>(bodyData, "Success", true);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occurred getting the heat map data by components");
                return new GetQueryResponse<IEnumerable<HeatMapByComponentsResponseVm>>([], "Failed", false);
            }

        }
    }
}
