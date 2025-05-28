using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public GetPollsByStudentQueryHandler(IStudentPollsRepository repository, ILogger<GetPollsByStudentQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<List<GetPollsQueryResponse>> Handle(GetPollsByStudentQuery request, CancellationToken cancellationToken)
        {
            var listOfPolls = await _repository.GetPollsByStudentIdAsync(request.StudentId);
            var pollsResponses = listOfPolls.Select(poll => new GetPollsQueryResponse
            {
                Id = poll.Id,
                Uuid = poll.Uuid,
                Name = poll.Name,
                LastVersion = poll.LastVersion,
                LastVersionDate = poll.LastVersionDate,
            }).ToList();


            return pollsResponses;
        }

    }
}
