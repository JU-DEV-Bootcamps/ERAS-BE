using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Interventions.Queries.GetInterventions;
public class GetInterventionsQueryHandler : IRequestHandler<GetInterventionsQuery, PagedResult<JUIntervention>>
{
    private readonly IInterventionRepository _interventionRepository;
    private readonly ILogger<GetInterventionsQueryHandler> _logger;

    public GetInterventionsQueryHandler(IInterventionRepository InterventionRepository, ILogger<GetInterventionsQueryHandler> Logger)
    {
        _interventionRepository = InterventionRepository;
        _logger = Logger;
    }

    public async Task<PagedResult<JUIntervention>> Handle(GetInterventionsQuery Request, CancellationToken CancellationToken)
    {
        try
        {
            var interventions = await _interventionRepository.GetPagedAsync(Request.Query.Page, Request.Query.PageSize);
            var totalCount = await _interventionRepository.CountAsync();
            PagedResult<JUIntervention> pagedResult = new PagedResult<JUIntervention>(
                totalCount,
                interventions.ToList()
            );

            return pagedResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting interventions: " + ex.Message);
            return new PagedResult<JUIntervention>(0, []);
        }
    }
}
