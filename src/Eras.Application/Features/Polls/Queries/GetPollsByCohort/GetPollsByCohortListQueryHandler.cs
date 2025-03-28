﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Polls.Commands.CreatePoll;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Polls.Queries.GetPollsByCohort
{
    public class GetPollsByCohortListQueryHandler : IRequestHandler<GetPollsByCohortListQuery, List<Poll>>
    {
        private readonly IPollCohortRepository _repository;
        private readonly ILogger<GetPollsByCohortListQueryHandler> _logger;

        public GetPollsByCohortListQueryHandler( IPollCohortRepository pollCohortRepository, ILogger<GetPollsByCohortListQueryHandler> logger)
        {
            _repository = pollCohortRepository;
            _logger = logger;
        }

        public async Task<List<Poll>> Handle(GetPollsByCohortListQuery request, CancellationToken cancellationToken)
        {
            var listOfPolls = await _repository.GetPollsByCohortIdAsync(request.CohortId);
            return listOfPolls;
        }

    }
}
