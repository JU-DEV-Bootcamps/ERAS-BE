using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Components.Queries;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Answers.Queries
{
    public class GetStudentAnswersByPollQueryHandler : IRequestHandler<GetStudentAnswersByPollQuery, PagedResult<StudentAnswer>>
    {
        private readonly IStudentAnswersRepository _studentAnswersRepository;
        private readonly ILogger<GetStudentAnswersByPollQueryHandler> _logger;

        public GetStudentAnswersByPollQueryHandler(IStudentAnswersRepository StudentAnswersRepository, ILogger<GetStudentAnswersByPollQueryHandler> Logger)
        {
            _studentAnswersRepository = StudentAnswersRepository;
            _logger = Logger;
        }

        public async Task<PagedResult<StudentAnswer>> Handle(GetStudentAnswersByPollQuery Request, CancellationToken CancellationToken)
        {
            var answers = await _studentAnswersRepository.GetStudentAnswersPagedAsync(Request.StudentId, Request.PollId, Request.Query.Page, Request.Query.PageSize);
            
            return answers;
        }
    }
}
