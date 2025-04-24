using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Components.Queries;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Answers.Queries
{
    public class GetStudentAnswersByPollQueryHandler : IRequestHandler<GetStudentAnswersByPollQuery, List<StudentAnswer>>
    {
        private readonly IStudentAnswersRepository _studentAnswersRepository;
        private readonly ILogger<GetStudentAnswersByPollQueryHandler> _logger;

        public GetStudentAnswersByPollQueryHandler(IStudentAnswersRepository StudentAnswersRepos00itory, ILogger<GetStudentAnswersByPollQueryHandler> logger)
        {
            _studentAnswersRepository = StudentAnswersRepos00itory;
            _logger = logger;
        }

        public async Task<List<StudentAnswer>> Handle(GetStudentAnswersByPollQuery Request, CancellationToken CancellationToken)
        {
            var listOfAnswers = await _studentAnswersRepository.GetStudentAnswersAsync(Request.StudentId, Request.PollId);
            return listOfAnswers;
        }
    }
}
