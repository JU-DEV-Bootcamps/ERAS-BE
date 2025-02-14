using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.GetByRulesetQuery;

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
            var answers = await _answerRepository.GetByIdAsync(request.RulesetVariablesWeight.FirstOrDefault().Key.Id);
            return new BaseResponse(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating the Ruleset: " + request);
            return new BaseResponse(false);
        }
    }
}
