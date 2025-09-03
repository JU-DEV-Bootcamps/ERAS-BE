using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.JUServices.Queries.GetJUServices
{
    public class GetJUServicesQueryHandler
        : IRequestHandler<GetJUServicesQuery, List<JUService>>
    {
        private readonly IJUServiceRepository _serviceRepository;
        private readonly ILogger<GetJUServicesQuery> _logger;
            
        public GetJUServicesQueryHandler(IJUServiceRepository ServiceRepository, ILogger<GetJUServicesQuery> Logger)
        {
            _serviceRepository = ServiceRepository;
            _logger = Logger;
        }

        public async Task<List<JUService>> Handle(GetJUServicesQuery Request, CancellationToken CancellationToken)
        {
            try
            {
                var services = await _serviceRepository.GetAllAsync();

                return services.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting polls: " + ex.Message);
                return new List<JUService>();
            }
        }
    }
}