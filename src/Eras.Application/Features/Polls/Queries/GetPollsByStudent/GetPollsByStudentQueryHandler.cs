using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Polls.Queries.GetPollsByCohort;
using Eras.Application.Models.Response.Controllers.PollsController;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Polls.Queries.GetPollsByStudent
{
    public class GetPollsByStudentQueryHandler : IRequestHandler<GetPollsByStudentQuery, List<GetPollsQueryResponse>>
    {
        private readonly IStudentPollsRepository _repository;
        private readonly ILogger<GetPollsByStudentQueryHandler> _logger;

        public GetPollsByStudentQueryHandler(IStudentPollsRepository Repository, ILogger<GetPollsByStudentQueryHandler> Logger)
        {
            _repository = Repository;
            _logger = Logger;
        }

        public async Task<List<GetPollsQueryResponse>> Handle(GetPollsByStudentQuery Request, CancellationToken CancellationToken)
        {
            var listOfPolls = await _repository.GetPollsByStudentIdAsync(Request.StudentId);
            var pollsResponses = listOfPolls.Select(Poll => new GetPollsQueryResponse
            {
                Id = Poll.Id,
                Uuid = Poll.Uuid,
                Name = Poll.Name,
                LastVersion = Poll.LastVersion,
                LastVersionDate = Poll.LastVersionDate,
            }).ToList();

            return pollsResponses;
        }
    }
}
