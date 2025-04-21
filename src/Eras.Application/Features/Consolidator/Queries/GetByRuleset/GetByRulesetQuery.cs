using Eras.Application.Models.Response;
using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.GetByRuleset;

public class GetByRulesetQuery: IRequest<BaseResponse>
{
    public required List<(int AnswerId, int Weight)> RulesetVariablesWeight { get; set; }

}
