using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Students.Queries.GetAllByPollAndDate
{
    public class GetAllStudentsByPollUuidAndDaysQuery : IRequest<PagedResult<Student>>
    {
        public required Pagination Query { get; set; }
        public required string PollUuid { get; set; }
        public int? Days { get; set; }
    }
}