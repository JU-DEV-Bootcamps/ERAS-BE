using Eras.Application.Contracts.Persistence;
using Eras.Application.Exceptions;
using Eras.Application.Models;
using Eras.Application.Models.Consolidator;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class PollAvgHandler(
    ILogger<PollAvgHandler> Logger,
    IComponentRepository CompRepository,
    IPollInstanceRepository PollInstanceRepository
    ) : IRequestHandler<PollAvgQuery, GetQueryResponse<AvgConsolidatorResponseVm>>
{
    private readonly IComponentRepository _compRepo = CompRepository;
    private readonly IPollInstanceRepository _pollInstanceRepository = PollInstanceRepository;
    private readonly ILogger<PollAvgHandler> _logger = Logger;

    public async Task<GetQueryResponse<AvgConsolidatorResponseVm>> Handle(PollAvgQuery Req, CancellationToken CancToken)
    {
        try
        {
            var answersByFilters = await _pollInstanceRepository.GetByUuidAsync(Req.PollUuid.ToString())
                ?? throw new NotFoundException($"Error in query for filters: {Req.PollUuid}");

            if (answersByFilters == null) // Returns empty response
                return new GetQueryResponse<AvgConsolidatorResponseVm>(new AvgConsolidatorResponseVm(), "Success: No answered polls with that Uuid", true);

            var mappedReport = Mappe
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
