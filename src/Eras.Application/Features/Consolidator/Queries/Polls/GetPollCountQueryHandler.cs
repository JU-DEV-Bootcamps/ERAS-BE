using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Consolidator;
using Eras.Application.Models.Response.Common;
using Eras.Error.Bussiness;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class PollCountQueryHandler(
    ILogger<PollCountQueryHandler> Logger,
    IPollInstanceRepository PollInstanceRepository,
    IEvaluationRepository EvaluationRepository 
    ) : IRequestHandler<PollCountQuery, GetQueryResponse<CountReportResponseVm>>
{
    private readonly IPollInstanceRepository _pollInstanceRepository = PollInstanceRepository;
    private readonly IEvaluationRepository _evaluationRepository = EvaluationRepository;
    private readonly ILogger<PollCountQueryHandler> _logger = Logger;

    public async Task<GetQueryResponse<CountReportResponseVm>> Handle(PollCountQuery Req, CancellationToken CancToken)
    {
        try
        {
            var evaluation = await _evaluationRepository.GetByIdAsync(Req.EvaluationId);

            if (evaluation == null)
            {
                return new GetQueryResponse<CountReportResponseVm>(new CountReportResponseVm(), "Failed: Evaluation not found", false);
            }

            var startDate = DateTime.SpecifyKind(evaluation.StartDate, DateTimeKind.Utc);
            var endDate = DateTime.SpecifyKind(evaluation.EndDate, DateTimeKind.Utc)
                                .Date
                                .AddDays(1)
                                .AddTicks(-1); 

            var answersByFilters = await _pollInstanceRepository.GetCountReportByVariablesAsync(Req.PollUuid, Req.CohortIds, Req.VariableIds, Req.LastVersion, startDate, endDate, Req.EvaluationId)
                ?? throw new NotFoundException($"Error in query for filters: {Req.PollUuid}; {Req.CohortIds}");

            if (answersByFilters == null) 
                return new GetQueryResponse<CountReportResponseVm>(new CountReportResponseVm(), "Success: No answered polls with that Uuid", true);
            return new GetQueryResponse<CountReportResponseVm>(answersByFilters, "Success", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred getting the heat map summary data by filters");
            return new GetQueryResponse<CountReportResponseVm>(new CountReportResponseVm(), "Failed: " + ex.Message, false);
        }
    }
}
