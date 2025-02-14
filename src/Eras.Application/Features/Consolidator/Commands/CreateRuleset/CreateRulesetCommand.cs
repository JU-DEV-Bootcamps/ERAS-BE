using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;
namespace Eras.Application.Features.Consolidator.Commands.CreateRuleset;

public class CreateRulesetCommand: IRequest<BaseResponse>
{
    public required Dictionary<Variable, int> RulesetVariablesWeight { get; set; }
}
