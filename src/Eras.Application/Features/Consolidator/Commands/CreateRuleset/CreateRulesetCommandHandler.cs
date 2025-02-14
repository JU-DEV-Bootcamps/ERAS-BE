using System;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Commands.CreateRuleset;

public class CreateRulesetCommandHandler: IRequestHandler<CreateRulesetCommand, BaseResponse>
{
    private readonly IAnswerRepository _answerRepository;
    private readonly ILogger<CreateRulesetCommandHandler> _logger;

    public CreateRulesetCommandHandler(IAnswerRepository answerRepository, ILogger<CreateRulesetCommandHandler> logger)
    {
        _answerRepository = answerRepository;
        _logger = logger;
    }

    public async Task<BaseResponse> Handle(CreateRulesetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //TODO: Create Ruleset repository to save variable ids and weights to the database
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
