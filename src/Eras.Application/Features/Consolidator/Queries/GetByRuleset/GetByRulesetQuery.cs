using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.GetByRulesetQuery;

public class GetByRulesetQuery: IRequest<BaseResponse>
{
    public required Dictionary<Variable, int> RulesetVariablesWeight { get; set; }

}
