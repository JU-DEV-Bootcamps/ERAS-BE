using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Remmisions.Queries.GetRemissions
{
    public class GetRemissionByIdQueryHandler
        : IRequestHandler<GetRemissionByIdQuery, JURemission>
    {
        private readonly IRemissionRepository _remissionRepository;
        private readonly ILogger<GetRemissionByIdQueryHandler> _logger;

        public GetRemissionByIdQueryHandler(IRemissionRepository RemissionRepository,
            ILogger<GetRemissionByIdQueryHandler> Logger)
        {
            _remissionRepository = RemissionRepository;
            _logger = Logger;
        }

        public async Task<JURemission> Handle(GetRemissionByIdQuery Request, CancellationToken CancellationToken)
        {
            try
            {
                var remission = await _remissionRepository.GetByIdAsync(Request.Id);

                return remission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting polls: " + ex.Message);
                return new JURemission();
            }
        }
    }
        
}