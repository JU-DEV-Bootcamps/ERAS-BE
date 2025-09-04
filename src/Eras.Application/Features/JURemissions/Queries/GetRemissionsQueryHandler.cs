using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Remmisions.Queries.GetRemissions
{
    public class GetRemissionsQueryHandler
        : IRequestHandler<GetRemissionsQuery, List<JURemission>>
    {
        private readonly IRemissionRepository _remissionRepository;
        private readonly ILogger<GetRemissionsQuery> _logger;
            
        public GetRemissionsQueryHandler(IRemissionRepository RemissionRepository, ILogger<GetRemissionsQuery> Logger)
        {
            _remissionRepository = RemissionRepository;
            _logger = Logger;
        }

        public async Task<List<JURemission>> Handle(GetRemissionsQuery Request, CancellationToken CancellationToken)
        {
            try
            {
                var remissions = await _remissionRepository.GetPagedAsync(Request.Query.Page, Request.Query.PageSize);

                return remissions.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting remissions: " + ex.Message);
                return new List<JURemission>();
            }
        }
    }
}