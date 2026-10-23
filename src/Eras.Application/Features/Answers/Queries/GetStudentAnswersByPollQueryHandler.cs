using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Answers.Queries
{
    public class GetStudentAnswersByPollQueryHandler : IRequestHandler<GetStudentAnswersByPollQuery, PagedResult<StudentAnswer>>
    {
        private readonly IStudentAnswersRepository _studentAnswersRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<GetStudentAnswersByPollQueryHandler> _logger;

        public GetStudentAnswersByPollQueryHandler(
            IStudentAnswersRepository StudentAnswersRepository,
            IStudentRepository StudentRepository,
            ILogger<GetStudentAnswersByPollQueryHandler> Logger)
        {
            _studentAnswersRepository = StudentAnswersRepository;
            _studentRepository = StudentRepository;
            _logger = Logger;
        }

        public async Task<PagedResult<StudentAnswer>> Handle(GetStudentAnswersByPollQuery Request, CancellationToken CancellationToken)
        {
            if (Request.PollId <= 0 || Request.StudentId <= 0)
                throw new ArgumentException("PollId and StudentId must be positive integers.");

            if (Request.Query == null)
                throw new ArgumentNullException("Request must contain pagination");

            var student = await _studentRepository.GetByIdAsync(Request.StudentId);
            if (student == null)
                throw new KeyNotFoundException($"Student with ID {Request.StudentId} not found.");

            return await _studentAnswersRepository.GetStudentAnswersPagedAsync(
                Request.StudentId, Request.PollId, Request.Query.Page, Request.Query.PageSize);
        }
    }
}
