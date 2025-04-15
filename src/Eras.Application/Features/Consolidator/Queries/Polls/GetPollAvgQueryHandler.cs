using Eras.Application.Contracts.Persistence;
using Eras.Application.Exceptions;
using Eras.Application.Mappers;
using Eras.Application.Models;
using Eras.Application.Models.Consolidator;

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
            IEnumerable<Domain.Entities.Answer> answersByFilters = await _pollInstanceRepository.GetAnswersByPollInstanceUuidAsync(Req.PollUuid.ToString())
                ?? throw new NotFoundException($"Error in query for filters: {Req.PollUuid}");

            if (answersByFilters == null) // Returns empty response
                return new GetQueryResponse<AvgReportResponseVm>(new AvgReportResponseVm(), "Success: No answered polls with that Uuid", true);
            AvgReportResponseVm avgRes = ReportMapper.MaptToVmResponse(answersByFilters);
            return new GetQueryResponse<AvgReportResponseVm>(avgRes, "Success", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred getting the heat map summary data by filters");
            return new GetQueryResponse<AvgReportResponseVm>(new AvgReportResponseVm(), "Failed", false);
        }
    }
}
