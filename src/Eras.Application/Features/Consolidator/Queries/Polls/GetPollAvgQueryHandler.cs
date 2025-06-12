using Eras.Application.Contracts.Persistence;
using Eras.Application.Exceptions;
using Eras.Application.Models.Consolidator;
using Eras.Application.Models.Response.Common;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class PollAvgHandler(
    ILogger<PollAvgHandler> Logger,
    IPollInstanceRepository PollInstanceRepository
    ) : IRequestHandler<PollAvgQuery, GetQueryResponse<AvgReportResponseVm>>
{
    private readonly IPollInstanceRepository _pollInstanceRepository = PollInstanceRepository;
    private readonly ILogger<PollAvgHandler> _logger = Logger;

    public async Task<GetQueryResponse<AvgReportResponseVm>> Handle(PollAvgQuery Request, CancellationToken CancellationToken)
    {
        try
        {
            var answersByFilters = await _pollInstanceRepository.GetReportByPollCohortAsync(Request.PollUuid.ToString(), Request.CohortIds, Request.LastVersion)
                ?? throw new NotFoundException($"Error in query for filters: {Request.PollUuid}; {Request.CohortIds}");

            if (answersByFilters == null) // Returns empty response
                return new GetQueryResponse<AvgReportResponseVm>(new AvgReportResponseVm(), "Success: No answered polls with that Uuid", true);
            return new GetQueryResponse<AvgReportResponseVm>(answersByFilters, "Success", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred getting the heat map summary data by filters");
            return new GetQueryResponse<AvgReportResponseVm>(new AvgReportResponseVm(), "Failed: " + ex.Message, false);
        }
    }
}
