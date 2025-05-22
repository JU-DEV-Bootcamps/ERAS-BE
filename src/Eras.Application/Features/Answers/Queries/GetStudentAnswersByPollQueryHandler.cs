using Eras.Application.Contracts.Persistence;
using Eras.Application.Exceptions;
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
            if (Request == null)
                throw new ArgumentNullException(nameof(Request));

            if (Request.PollId <= 0 || Request.StudentId <= 0)
                throw new ArgumentException("PollId and StudentId must be positive integers.");

            if (Request.Query == null)
                throw new ArgumentNullException("Request must contain pagination");

            var answers = await _studentAnswersRepository.GetStudentAnswersPagedAsync(Request.StudentId, Request.PollId, Request.Query.Page, Request.Query.PageSize);
            
            return answers;
        }
    }
}
