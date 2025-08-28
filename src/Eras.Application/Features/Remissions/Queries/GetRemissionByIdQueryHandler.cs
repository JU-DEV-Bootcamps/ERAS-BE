using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Remmisions.Queries.GetRemissions
{
    public class GetRemissionByIdQueryHandler
        : IRequestHandler<GetRemissionByIdQuery, PagedResult<GetRemissionByIdQueryResponse>>
    {
        private readonly IRemissionRepository _remissionRepository;
        private readonly ILogger<GetRemissionByIdQueryHandler> _logger;
            
        public GetRemissionByIdQueryHandler(IRemissionRepository RemissionRepository,
            ILogger<GetRemissionByIdQueryHandler> Logger) 
            { 
            _remissionRepository = RemissionRepository;
            _logger = Logger;
            }
        }
}