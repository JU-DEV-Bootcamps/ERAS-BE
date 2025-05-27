using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Polls.Queries.GetPollsByStudent
{
    public class GetPollsByStudentQueryHandler : IRequestHandler<GetPollsByStudentQuery, List<Poll>>
    {
        private readonly IStudentPollsRepository _repository;
        private readonly ILogger<GetPollsByStudentQueryHandler> _logger;

        public GetPollsByStudentQueryHandler(IStudentPollsRepository Repository, ILogger<GetPollsByStudentQueryHandler> Logger)
        {
            _repository = Repository;
            _logger = Logger;
        }

        public async Task<List<Poll>> Handle(GetPollsByStudentQuery Request, CancellationToken CancellationToken)
        {
            var listOfPolls = await _repository.GetPollsByStudentIdAsync(Request.StudentId);
            return listOfPolls;
        }

    }
}
