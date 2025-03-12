using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapSummary;
using Eras.Application.Models.HeatMap;
using Eras.Application.Models;
using MediatR;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents;
using Microsoft.Extensions.Logging;
using Eras.Application.Exceptions;
using Eras.Application.Mappers;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapSummaryByFilters
{
    internal class GetHeatMapSummaryByFiltersHandler 
        : IRequestHandler<GetHeatMapSummaryByFiltersQuery, GetQueryResponse<HeatMapSummaryResponseVm>>
    {
        private readonly IHeatMapRepository _heatMapRepository;
        private readonly IComponentRepository _componentRepository;
        private readonly ILogger<GetHeatMapSummaryByFiltersHandler> _logger;

        public GetHeatMapSummaryByFiltersHandler(
            IHeatMapRepository heatMapRepository, 
            IComponentRepository componentRepository, 
            ILogger<GetHeatMapSummaryByFiltersHandler> logger)
        {
            _heatMapRepository = heatMapRepository;
            _componentRepository = componentRepository;
            _logger = logger;
        }

        public async Task<GetQueryResponse<HeatMapSummaryResponseVm>> Handle(
            GetHeatMapSummaryByFiltersQuery request, 
            CancellationToken cancellationToken)
        {
            try
            {
                var answersByFilters = await _heatMapRepository.GetHeatMapDataByCohortAndDaysAsync(request.CohortId, request.Days) 
                    ?? throw new NotFoundException($"Error in query for filters: {request.CohortId} - {request.Days}");
                
                if (!answersByFilters.Any()) // Returns empty response
                    return new GetQueryResponse<HeatMapSummaryResponseVm>(new HeatMapSummaryResponseVm(), "Success", true);

                var mappedData = HeatMapMapper.MapToSummaryVmResponse(answersByFilters);

                return new GetQueryResponse<HeatMapSummaryResponseVm>(mappedData, "Success", true);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "An error occurred getting the heat map summary data by filters");
                return new GetQueryResponse<HeatMapSummaryResponseVm>(body: null, "Failed", false);
            }
        }
    }
}
