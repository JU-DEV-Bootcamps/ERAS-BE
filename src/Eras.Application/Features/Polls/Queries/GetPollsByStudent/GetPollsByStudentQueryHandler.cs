using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Polls.Queries.GetPollsByCohort;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Polls.Queries.GetPollsByStudent
{
    public class GetPollsByStudentQueryHandler : IRequestHandler<GetPollsByStudentQuery, List<Poll>>
    {
        private readonly IStudentPollsRepository _repository;
        private readonly ILogger<GetPollsByStudentQueryHandler> _logger;

        public GetPollsByStudentQueryHandler(IStudentPollsRepository repository, ILogger<GetPollsByStudentQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<List<Poll>> Handle(GetPollsByStudentQuery request, CancellationToken cancellationToken)
        {
            var listOfPolls = await _repository.GetPollsByStudentIdAsync(request.StudentId);
            return listOfPolls;
        }

    }
}
