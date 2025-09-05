using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Remmisions.Queries.GetRemissions
{
    public class GetRemissionsQueryHandler
        : IRequestHandler<GetRemissionsQuery, PagedResult<JURemission>>
    {
        private readonly IRemissionRepository _remissionRepository;
        private readonly ILogger<GetRemissionsQuery> _logger;
            
        public GetRemissionsQueryHandler(IRemissionRepository RemissionRepository, ILogger<GetRemissionsQuery> Logger)
        {
            _remissionRepository = RemissionRepository;
            _logger = Logger;
        }

        public async Task<PagedResult<JURemission>> Handle(GetRemissionsQuery Request, CancellationToken CancellationToken)
        {
            try
            {
                var remissions = await _remissionRepository.GetPagedAsync(Request.Query.Page, Request.Query.PageSize);
                var totalCount = await _remissionRepository.CountAsync();
                PagedResult<JURemission> pagedResult = new PagedResult<JURemission>(
                    totalCount,
                    remissions.ToList()
                );

                return pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting remissions: " + ex.Message);
                return new PagedResult<JURemission>(0, []);;
            }
        }
    }
}