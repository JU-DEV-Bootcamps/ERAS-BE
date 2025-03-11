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

        public GetStudentAnswersByPollQueryHandler(IStudentAnswersRepository studentAnswersRepository, ILogger<GetStudentAnswersByPollQueryHandler> logger)
        {
            _studentAnswersRepository = studentAnswersRepository;
            _logger = logger;
        }

        public async Task<List<StudentAnswer>> Handle(GetStudentAnswersByPollQuery request, CancellationToken cancellationToken)
        {
            var listOfAnswers = await _studentAnswersRepository.GetStudentAnswersAsync(request.StudentId, request.PollId);
            return listOfAnswers;
        }
    }
}
