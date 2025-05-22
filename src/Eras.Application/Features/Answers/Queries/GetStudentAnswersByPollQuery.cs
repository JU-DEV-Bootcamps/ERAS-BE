using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Answers.Queries
{
    public class GetStudentAnswersByPollQuery() : IRequest<PagedResult<StudentAnswer>>
    {
        public int StudentId { get; set; }
        public int PollId { get; set; }

        public Pagination Query { get; set; } = default!;
    }
}
