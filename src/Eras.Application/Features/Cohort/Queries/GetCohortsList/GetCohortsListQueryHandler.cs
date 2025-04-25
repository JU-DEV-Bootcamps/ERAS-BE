using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohort.Queries.GetCohortsList
{
    public class GetCohortsListQueryHandler : IRequestHandler<GetCohortsListQuery, List<Domain.Entities.Cohort>>
    {
        private readonly ICohortRepository _repository;
        private readonly ILogger<GetCohortsListQueryHandler> _logger;

        public GetCohortsListQueryHandler(ICohortRepository Repository, ILogger<GetCohortsListQueryHandler> Logger)
        {
            _repository = Repository;
            _logger = Logger;
        }

        public async Task<List<Domain.Entities.Cohort>> Handle(GetCohortsListQuery Request, CancellationToken cancellationToken)
        {
            var listOfCohorts = await _repository.GetCohortsAsync();
            return listOfCohorts;
        }
    }
}
