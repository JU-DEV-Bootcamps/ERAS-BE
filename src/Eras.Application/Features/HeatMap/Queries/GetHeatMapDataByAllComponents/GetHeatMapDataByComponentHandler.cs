using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Microsoft.Extensions.Logging;
using MediatR;
using Eras.Application.Exceptions;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Response.HeatMap;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents
{
    public class GetHeatMapDataByAllComponentsHandler : IRequestHandler<GetHeatMapDataByAllComponentsQuery, GetQueryResponse<IEnumerable<HeatMapByComponentsResponseVm>>>
    {
        private readonly IHeatMapRepository _heatMapRepository;
        private readonly IComponentRepository _componentRepository;
        private readonly ILogger<GetHeatMapDataByAllComponentsHandler> _logger;

        public GetHeatMapDataByAllComponentsHandler(IHeatMapRepository heatMapRepository, IComponentRepository componentRepository , ILogger<GetHeatMapDataByAllComponentsHandler> logger)
        {
            _heatMapRepository = heatMapRepository;
            _componentRepository = componentRepository;
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
                var components = await _componentRepository.GetAllAsync();
                if (answersByComponents == null || !answersByComponents.Any())
                    throw new NotFoundException($"No data found for poll instance ID: {request.PollInstanceUUID}");

                var bodyData = new List<HeatMapByComponentsResponseVm>();

                foreach (var component in components)
                {
                    var filteredResponses = answersByComponents
                        .Where(q => q.ComponentId == component.Id)
                        .ToList();
                    var mappedData = HeatMapMapper.MaptToVmResponse(filteredResponses, component.Name);
                    bodyData.Add(mappedData);
                }

                return new GetQueryResponse<IEnumerable<HeatMapByComponentsResponseVm>>(bodyData, "Success", true);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occurred getting the heat map data by components");
                return new GetQueryResponse<IEnumerable<HeatMapByComponentsResponseVm>>([], "Failed", false);
            }

        }
    }
}
