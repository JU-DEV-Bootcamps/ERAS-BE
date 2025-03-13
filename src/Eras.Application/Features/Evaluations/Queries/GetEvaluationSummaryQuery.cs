using MediatR;
using Eras.Domain.Entities;

namespace Eras.Application.Features.Evaluations.Queries
{
    public class GetEvaluationSummaryQuery: IRequest<List<Evaluation>>;
}
