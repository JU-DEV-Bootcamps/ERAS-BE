using Eras.Application.Contracts.Persistence;
using Eras.Application.Exceptions;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents;
using Eras.Application.Mappers;
using Eras.Application.Models;
using Eras.Application.Models.HeatMap;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByVariables
{
    public class GetHeatMapDataByVariablesHandler : IRequestHandler<GetHeatMapDataByVariablesQuery, GetQueryResponse<HeatMapByVariablesResponseVm>>
    {
        private readonly IHeatMapRepository _heatMapRepository;
        private readonly ILogger<GetHeatMapDataByAllComponentsHandler> _logger;

        public GetHeatMapDataByVariablesHandler(IHeatMapRepository heatMapRepository, ILogger<GetHeatMapDataByAllComponentsHandler> logger)
        {
            _heatMapRepository = heatMapRepository;
            _logger = logger;
        }
        public async Task<GetQueryResponse<HeatMapByVariablesResponseVm>> Handle(GetHeatMapDataByVariablesQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.PollInstanceUUID))
            {
                throw new NotFoundException($"Poll instance ID cannot be null or empty");
            }
            try
            {
                var answersByVarialbe = await _heatMapRepository.GetHeatMapDataByVariables(request.Component, request.PollInstanceUUID);
                if (answersByVarialbe == null || !answersByVarialbe.Any())
                    throw new NotFoundException($"No data found for poll instance ID: {request.PollInstanceUUID}");

                var bodyData = HeatMapMapper.MapToVmResponse(answersByVarialbe);
                return new GetQueryResponse<HeatMapByVariablesResponseVm>(bodyData, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred getting the heat map data by components");
                return new GetQueryResponse<HeatMapByVariablesResponseVm>(new HeatMapByVariablesResponseVm(), "Failed", false);
            }
        }
    }
}
