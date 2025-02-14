using System;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.GetAvgRiskAnswer;

public class GetAvgRiskAnswerQueryHandler: IRequestHandler<GetAvgRiskAnswerQuery, BaseResponse>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly ILogger<GetAvgRiskAnswerQuery> _logger;

    public GetAvgRiskAnswerQueryHandler(IAnswerRepository answerRepository, ILogger<GetAvgRiskAnswerQuery> logger)
    {
        _answerRepository = answerRepository;
        _logger = logger;
    }

    public async Task<BaseResponse> Handle(GetAvgRiskAnswerQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var answers = await _answerRepository.GetByIdAsync(request.Answer.Id);
            //TODO: Implement logic to calculate the average risk answer
            return new BaseResponse(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating the Ruleset: " + request);
            return new BaseResponse(false);
        }
    }
}
