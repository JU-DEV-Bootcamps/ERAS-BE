using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.JUServices.Queries.GetJUServices
{
    public class GetJUServicesQueryHandler
        : IRequestHandler<GetJUServicesQuery, PagedResult<JUService>>
    {
        private readonly IJUServiceRepository _serviceRepository;
        private readonly ILogger<GetJUServicesQuery> _logger;
            
        public GetJUServicesQueryHandler(IJUServiceRepository ServiceRepository, ILogger<GetJUServicesQuery> Logger)
        {
            _serviceRepository = ServiceRepository;
            _logger = Logger;
        }

        public async Task<PagedResult<JUService>> Handle(GetJUServicesQuery Request, CancellationToken CancellationToken)
        {
            try
            {
                var services = await _serviceRepository.GetAllAsync();
                var totalCount = await _serviceRepository.CountAsync();
                PagedResult<JUService> pagedResult = new PagedResult<JUService>(
                    totalCount,
                    services.ToList()
                );

                return pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting Services: " + ex.Message);
                return new PagedResult<JUService>(0, []);
            }
        }
    }
}