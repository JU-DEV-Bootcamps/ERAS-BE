using MediatR;
using Eras.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;
using Eras.Application.Exceptions;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Response.HeatMap;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapSummaryByFilters
{
    public class GetHeatMapSummaryByFiltersHandler 
        : IRequestHandler<GetHeatMapSummaryByFiltersQuery, GetQueryResponse<HeatMapSummaryResponseVm>>
    {
        private readonly IHeatMapRepository _heatMapRepository;
        private readonly ILogger<GetHeatMapSummaryByFiltersHandler> _logger;

        public GetHeatMapSummaryByFiltersHandler(
            IHeatMapRepository HeatMapRepository, 
            ILogger<GetHeatMapSummaryByFiltersHandler> Logger)
        {
            _heatMapRepository = HeatMapRepository;
            _logger = Logger;
        }

        public async Task<GetQueryResponse<HeatMapSummaryResponseVm>> Handle(
            GetHeatMapSummaryByFiltersQuery Request, 
            CancellationToken CancellationToken)
        {
            try
            {
                var answersByFilters = await _heatMapRepository.GetHeatMapDataByCohortAndDaysAsync(Request.CohortId, Request.Days) 
                    ?? throw new NotFoundException($"Error in query for filters: {Request.CohortId} - {Request.Days}");
                
                if (!answersByFilters.Any()) // Returns empty response
                    return new GetQueryResponse<HeatMapSummaryResponseVm>(new HeatMapSummaryResponseVm(), "Success", true);

                var mappedData = HeatMapMapper.MapToSummaryVmResponse(answersByFilters);

                return new GetQueryResponse<HeatMapSummaryResponseVm>(mappedData, "Success", true);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "An error occurred getting the heat map summary data by filters");
                return new GetQueryResponse<HeatMapSummaryResponseVm>(Body: null, "Failed", false);
            }
        }
    }
}
