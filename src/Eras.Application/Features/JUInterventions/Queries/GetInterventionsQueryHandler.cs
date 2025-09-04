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
        try
            {
            var listOfInterventions = await _professionalRepository.GetPagedAsync(Request.Query.Page, Request.Query.PageSize);
            return listOfInterventions.ToList();
        }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting interventions: " + ex.Message);
                return new List<JUIntervention>();
            }
    }
}
