using Eras.Application.Contracts.Persistence;
using Eras.Application.Exceptions;
using Eras.Application.Mappers;
using Eras.Application.Models.Consolidator;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

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

    public async Task<GetQueryResponse<AvgReportResponseVm>> Handle(PollAvgQuery Req, CancellationToken CancToken)
    {
        try
        {
            var answersByFilters = await _pollInstanceRepository.GetAnswersByPollInstanceUuidAsync(Req.PollUuid.ToString(), Req.CohortId.ToString())
                ?? throw new NotFoundException($"Error in query for filters: {Req.PollUuid}; {Req.CohortId}");

            if (answersByFilters == null) // Returns empty response
                return new GetQueryResponse<AvgReportResponseVm>(new AvgReportResponseVm(), "Success: No answered polls with that Uuid", true);
            return new GetQueryResponse<AvgReportResponseVm>(answersByFilters, "Success", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred getting the heat map summary data by filters");
            return new GetQueryResponse<AvgReportResponseVm>(new AvgReportResponseVm(), "Failed", false);
        }
    }
}
