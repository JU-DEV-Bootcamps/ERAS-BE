using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;
namespace Eras.Application.Features.Evaluations.Queries.GetAll
{
    public sealed record GetAllEvaluationsQuery(Pagination Query) : IRequest<PagedResult<Evaluation>>
    {
    }
}
