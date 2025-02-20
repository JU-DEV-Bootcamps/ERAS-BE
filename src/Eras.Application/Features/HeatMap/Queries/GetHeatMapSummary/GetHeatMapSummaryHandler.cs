using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Models.HeatMap;
using Eras.Application.Models;
using MediatR;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents;
using Microsoft.Extensions.Logging;
using Eras.Application.Exceptions;
using Eras.Application.Mappers;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapSummary
{
    internal class GetHeatMapSummaryHandler : IRequestHandler<GetHeatMapSummaryQuery, GetQueryResponse<IEnumerable<HeatMapSummaryResponseVm>>>
    {
        private readonly IHeatMapRepository _heatMapRepository;
        private readonly IComponentRepository _componentRepository;
        private readonly ILogger<GetHeatMapDataByAllComponentsHandler> _logger;

        public GetHeatMapSummaryHandler(IHeatMapRepository heatMapRepository, IComponentRepository componentRepository, ILogger<GetHeatMapDataByAllComponentsHandler> logger)
        {
            _heatMapRepository = heatMapRepository;
            _componentRepository = componentRepository;
            _logger = logger;
        }

        public async Task<GetQueryResponse<IEnumerable<HeatMapSummaryResponseVm>>> Handle(GetHeatMapSummaryQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.PollInstanceUUID))
            {
                throw new NotFoundException($"Poll instance ID cannot be null or empty");
            }
            try {
                var answersByComponents = await _heatMapRepository.GetHeatMapDataByComponentsAsync(request.PollInstanceUUID);
                var components = await _componentRepository.GetAllAsync();
                if (answersByComponents == null || !answersByComponents.Any())
                    throw new NotFoundException($"No data found for poll instance ID: {request.PollInstanceUUID}");

                var bodyData = new List<HeatMapSummaryResponseVm>();

                var mappedData = HeatMapMapper.MapToSummaryVmResponse(answersByComponents);
                bodyData.Add(mappedData);

                return new GetQueryResponse<IEnumerable<HeatMapSummaryResponseVm>>(bodyData, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred getting the heat map data by components");
                return new GetQueryResponse<IEnumerable<HeatMapSummaryResponseVm>>([], "Failed", false);
            }
        }
    }
}
