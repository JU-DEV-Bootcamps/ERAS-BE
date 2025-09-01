using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Interventions.Queries.GetInterventions;
public class GetInterventionsQueryHandler : IRequestHandler<GetInterventionsQuery, List<JUIntervention>>
{
    private readonly IInterventionRepository _professionalRepository;
    private readonly ILogger<GetInterventionsQueryHandler> _logger;

    public GetInterventionsQueryHandler(IInterventionRepository InterventionRepository, ILogger<GetInterventionsQueryHandler> Logger)
    {
        _professionalRepository = InterventionRepository;
        _logger = Logger;
    }

    public async Task<List<JUIntervention>> Handle(GetInterventionsQuery Request, CancellationToken CancellationToken)
    {
        var listOfInterventions = await _professionalRepository.GetAllAsync();
        return listOfInterventions.ToList();
    }
}
