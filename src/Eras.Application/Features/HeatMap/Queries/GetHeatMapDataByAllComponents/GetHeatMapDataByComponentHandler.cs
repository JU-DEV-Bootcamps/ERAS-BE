using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Microsoft.Extensions.Logging;
using MediatR;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Response.HeatMap;
using Eras.Error.Bussiness;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents
{
    public class GetHeatMapDataByAllComponentsHandler : IRequestHandler<GetHeatMapDataByAllComponentsQuery, GetQueryResponse<IEnumerable<HeatMapByComponentsResponseVm>>>
    {
        private readonly IHeatMapRepository _heatMapRepository;
        private readonly IComponentRepository _componentRepository;
        private readonly ILogger<GetHeatMapDataByAllComponentsHandler> _logger;

        public GetHeatMapDataByAllComponentsHandler(IHeatMapRepository HeatMapRepository, IComponentRepository ComponentRepository , ILogger<GetHeatMapDataByAllComponentsHandler> Logger)
        {
            _heatMapRepository = HeatMapRepository;
            _componentRepository = ComponentRepository;
            _logger = Logger;
        }
        public async Task<GetQueryResponse<IEnumerable<HeatMapByComponentsResponseVm>>> Handle(GetHeatMapDataByAllComponentsQuery Request, CancellationToken CancellationToken)
        {
            if (string.IsNullOrEmpty(Request.PollInstanceUUID))
            {
                throw new NotFoundException($"Poll instance ID cannot be null or empty");
            }
            try
            {
                var answersByComponents = await _heatMapRepository.GetHeatMapDataByComponentsAsync(Request.PollInstanceUUID);
                var components = await _componentRepository.GetAllAsync();
                if (answersByComponents == null || !answersByComponents.Any())
                    throw new NotFoundException($"No data found for poll instance ID: {Request.PollInstanceUUID}");

                var bodyData = new List<HeatMapByComponentsResponseVm>();

                foreach (var component in components)
                {
                    var filteredResponses = answersByComponents
                        .Where(Q => Q.ComponentId == component.Id)
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
