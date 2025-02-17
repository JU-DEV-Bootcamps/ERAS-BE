using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.HeatMap;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents
{
    public class GetHeatMapDataByAllComponentsHandler : IRequestHandler<GetHeatMapDataByAllComponentsQuery, HeatMapByComponentsResponseVm>
    {
        private readonly IHeatMapRepository _heatMapRepository;
        private readonly ILogger<GetHeatMapDataByAllComponentsHandler> _logger;

        public GetHeatMapDataByAllComponentsHandler(IHeatMapRepository heatMapRepository, ILogger<GetHeatMapDataByAllComponentsHandler> logger)
        {
            _heatMapRepository = heatMapRepository;
            _logger = logger;
        }
        public async Task<HeatMapByComponentsResponseVm> Handle(GetHeatMapDataByAllComponentsQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var answersByComponents = await _heatMapRepository.GetHeatMapDataByComponentsAsync(request.PollInstanceUUID);
                var response = HeatMapMapper.MaptToVmResponse(answersByComponents);
                return response;
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occurred getting the heat map data by components");
                return new HeatMapByComponentsResponseVm();
            }

        }
    }
}
