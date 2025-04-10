using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Polls.Queries.GetAllPollsQuery
{
    public class GetAllPollsQueryHandler : IRequestHandler<GetAllPollsQuery, List<Poll>>
    {
        private readonly IPollRepository _pollRepository;
        private readonly ILogger<GetAllPollsQuery> _logger;

        public GetAllPollsQueryHandler(
            IPollRepository pollRepository,
            ILogger<GetAllPollsQuery> logger
        )
        {
            _pollRepository = pollRepository;
            _logger = logger;
        }

        public async Task<List<Poll>> Handle(GetAllPollsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return _pollRepository.GetAllAsync().Result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting polls: " + ex.Message);
                return new List<Poll>();
            }
        }
    }
}
