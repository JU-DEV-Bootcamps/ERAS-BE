using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Professionals.Queries.GetProfessionals;
public class GetProfessionalsQueryHandler : IRequestHandler<GetProfessionalsQuery, PagedResult<JUProfessional>>
{
    private readonly IProfessionalRepository _professionalRepository;
    private readonly ILogger<GetProfessionalsQueryHandler> _logger;

    public GetProfessionalsQueryHandler(IProfessionalRepository ProfessionalRepository, ILogger<GetProfessionalsQueryHandler> Logger)
    {
        _professionalRepository = ProfessionalRepository;
        _logger = Logger;
    }

    public async Task<PagedResult<JUProfessional>> Handle(GetProfessionalsQuery Request, CancellationToken CancellationToken)
    {
        try
        {
            var professionals = await _professionalRepository.GetPagedAsync(Request.Query.Page, Request.Query.PageSize);
            var totalCount = await _professionalRepository.CountAsync();
            PagedResult<JUProfessional> pagedResult = new PagedResult<JUProfessional>(
                totalCount,
                professionals.ToList()
            );

            return pagedResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting professionals: " + ex.Message);
            return new PagedResult<JUProfessional>(0, []);
        }
    }
}
