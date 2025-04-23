using Eras.Application.Contracts.Persistence;
using Eras.Application.Exceptions;
using Eras.Application.Mappers;
using Eras.Application.Models.Consolidator;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Response.HeatMap;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class VariableAvgQueryHandler(
    ILogger<VariableAvgQueryHandler> Logger,
    IAnswerRepository ARepo
    ) : IRequestHandler<VariableAvgQuery, GetQueryResponse<AvgReportResponseVm>>
{
    private readonly IAnswerRepository _answerRepository = ARepo;
    private readonly ILogger<VariableAvgQueryHandler> _logger = Logger;

    public async Task<GetQueryResponse<AvgReportResponseVm>> Handle(VariableAvgQuery Req, CancellationToken CancToken)
    {
        try
        {
            List<AnswersReportQueryResponse> answersByFilters = await _answerRepository.GetAnswersByPollVariablesAsync(Req.PollVariableIds)
                ?? throw new NotFoundException($"Error in query for filters: {Req.PollVariableIds}");
            AvgReportResponseVm report = ReportMapper.MapToVmResponse(answersByFilters);

            return new GetQueryResponse<AvgReportResponseVm>(report, "Success", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred getting the heat map summary data by filters");
            return new GetQueryResponse<AvgReportResponseVm>(new AvgReportResponseVm(), "An error occurred getting the heat map summary data by filters. " + ex.Message, false);
        }
    }
}
