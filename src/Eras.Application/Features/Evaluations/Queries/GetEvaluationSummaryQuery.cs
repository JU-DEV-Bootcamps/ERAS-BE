using Eras.Application.Models;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Evaluations.Queries
{
    public class GetEvaluationSummaryQuery : IRequest<QueryManyResponse<Evaluation>>;
}
