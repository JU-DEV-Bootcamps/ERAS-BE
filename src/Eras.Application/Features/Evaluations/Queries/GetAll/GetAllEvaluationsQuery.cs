using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Evaluations.Queries.GetAll;

public class GetAllEvaluationsQuery() : IRequest<PagedResult<Evaluation>>
{
    public Pagination Query { get; set; } = new Pagination();
}