using MediatR;
using Eras.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;
using Eras.Application.Exceptions;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Response.HeatMap;

namespace Eras.Application.Features.HeatMap.Queries.GetHeatMapSummary;

public class GetHeatMapSummaryHandler : IRequestHandler<GetHeatMapSummaryQuery, GetQueryResponse<HeatMapSummaryResponseVm>>
{
    private readonly IHeatMapRepository _heatMapRepository;
    private readonly ILogger<GetHeatMapSummaryHandler> _logger;

    public GetHeatMapSummaryHandler(
        IHeatMapRepository HeatMapRepository,
        ILogger<GetHeatMapSummaryHandler> Logger)
    {
        _heatMapRepository = HeatMapRepository;
        _logger = Logger;
    }

    public async Task<GetQueryResponse<HeatMapSummaryResponseVm>> Handle(GetHeatMapSummaryQuery Request, CancellationToken CancellationToken)
    {
        if (string.IsNullOrEmpty(Request.PollInstanceUUID))
        {
            throw new NotFoundException($"Poll instance ID cannot be null or empty");
        }
        try {
            IEnumerable<GetHeatMapAnswersPercentageByVariableQueryResponse> answersPercentage = await _heatMapRepository.GetHeatMapAnswersPercentageByVariableAsync(Request.PollInstanceUUID);

            HeatMapSummaryResponseVm mappedData = HeatMapMapper.MapToSummaryAndPercentageVmResponse(answersPercentage);

            return new GetQueryResponse<HeatMapSummaryResponseVm>(mappedData, "Success", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred getting the heat map summary data ");
            return new GetQueryResponse<HeatMapSummaryResponseVm>(body: new HeatMapSummaryResponseVm(), "Failed", false);
        }
    }
}
