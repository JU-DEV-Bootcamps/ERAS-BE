using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Professionals.Queries.GetProfessionals;
public class GetProfessionalsQueryHandler : IRequestHandler<GetProfessionalsQuery, List<JUProfessional>>
{
    private readonly IProfessionalRepository _professionalRepository;
    private readonly ILogger<GetProfessionalsQueryHandler> _logger;

    public GetProfessionalsQueryHandler(IProfessionalRepository ProfessionalRepository, ILogger<GetProfessionalsQueryHandler> Logger)
    {
        _professionalRepository = ProfessionalRepository;
        _logger = Logger;
    }

    public async Task<List<JUProfessional>> Handle(GetProfessionalsQuery Request, CancellationToken CancellationToken)
    {
        try
        {
            var listOfProfessionals = await _professionalRepository.GetAllAsync();
            return listOfProfessionals.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting professionals: " + ex.Message);
            return new List<JUProfessional>();
        }
    }
}
