using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.GetByRuleset;

public class GetByRulesetQuery: IRequest<BaseResponse>
{
    public required List<(int AnswerId, int Weight)> RulesetVariablesWeight { get; set; }

}
