using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Consolidator;
using Eras.Application.Models.Response.Common;
using Eras.Error.Bussiness;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class PollAvgHandler(
    ILogger<PollAvgHandler> Logger,
    IPollInstanceRepository PollInstanceRepository,
    IEvaluationRepository EvaluationRepository 
    ) : IRequestHandler<PollAvgQuery, GetQueryResponse<AvgReportResponseVm>>
{
    private readonly IPollInstanceRepository _pollInstanceRepository = PollInstanceRepository;
    private readonly IEvaluationRepository _evaluationRepository = EvaluationRepository; 
    private readonly ILogger<PollAvgHandler> _logger = Logger;

    public async Task<GetQueryResponse<AvgReportResponseVm>> Handle(PollAvgQuery Request, CancellationToken CancellationToken)
    {
        try
        {
            var evaluation = await _evaluationRepository.GetByIdAsync(Request.EvaluationId);
            var startDate = DateTime.SpecifyKind(evaluation.StartDate, DateTimeKind.Utc);
            var endDate = DateTime.SpecifyKind(evaluation.EndDate, DateTimeKind.Utc);

            var answersByFilters = await _pollInstanceRepository.GetReportByPollCohortAsync(Request.PollUuid.ToString(), Request.CohortIds, Request.LastVersion, startDate, endDate)
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
