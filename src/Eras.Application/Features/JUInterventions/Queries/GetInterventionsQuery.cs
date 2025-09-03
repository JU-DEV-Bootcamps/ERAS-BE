using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Interventions.Queries.GetInterventions;
public class GetInterventionsQuery : IRequest<List<JUIntervention>>
{
}
