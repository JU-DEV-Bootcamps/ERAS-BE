using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents;
using Microsoft.Extensions.Logging;
using Eras.Application.Exceptions;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Response.HeatMap;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapSummary
{
    public class GetHeatMapSummaryHandler : IRequestHandler<GetHeatMapSummaryQuery, GetQueryResponse<HeatMapSummaryResponseVm>>
    {
        private readonly IHeatMapRepository _heatMapRepository;
        private readonly ILogger<GetHeatMapSummaryHandler> _logger;

        public GetHeatMapSummaryHandler(
            IHeatMapRepository heatMapRepository,
            ILogger<GetHeatMapSummaryHandler> logger)
        {
            _heatMapRepository = heatMapRepository;
            _logger = logger;
        }

        public async Task<GetQueryResponse<HeatMapSummaryResponseVm>> Handle(GetHeatMapSummaryQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.PollInstanceUUID))
            {
                throw new NotFoundException($"Poll instance ID cannot be null or empty");
            }
            try {
                var answersPercentage = await _heatMapRepository.GetHeatMapAnswersPercentageByVariableAsync(request.PollInstanceUUID);

                var mappedData = HeatMapMapper.MapToSummaryAndPercentageVmResponse(answersPercentage);

                return new GetQueryResponse<HeatMapSummaryResponseVm>(mappedData, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred getting the heat map summary data ");
                return new GetQueryResponse<HeatMapSummaryResponseVm>(body: null, "Failed", false);
            }
        }
    }
}
