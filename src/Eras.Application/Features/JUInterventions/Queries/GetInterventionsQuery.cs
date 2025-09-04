using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Interventions.Queries.GetInterventions;

public class GetInterventionsQuery : IRequest<List<JUIntervention>>
{
    public Pagination Query { get; set; } = new Pagination();
}
