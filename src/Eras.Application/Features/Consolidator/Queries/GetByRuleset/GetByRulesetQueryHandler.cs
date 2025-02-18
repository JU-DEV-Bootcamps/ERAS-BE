using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.GetByRuleset;

public class GetByRulesetQueryHandler: IRequestHandler<GetByRulesetQuery, BaseResponse>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly ILogger<GetByRulesetQueryHandler> _logger;

    public GetByRulesetQueryHandler(IAnswerRepository answerRepository, ILogger<GetByRulesetQueryHandler> logger)
    {
        _answerRepository = answerRepository;
        _logger = logger;
    }

    public async Task<BaseResponse> Handle(GetByRulesetQuery request, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var (AnswerId, Weight) in request.RulesetVariablesWeight)
            {
                if(AnswerId != 0){
                    Answer answer = await _answerRepository.GetByIdAsync(AnswerId);
                    //TODO: Implement the logic to calculate the risk
                }
            }
            return new BaseResponse(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating the Ruleset: " + request);
            return new BaseResponse(false);
        }
    }
}
