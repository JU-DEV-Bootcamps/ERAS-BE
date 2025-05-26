using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Students.Queries.GetByEmail
{
    public class GetStudentByEmailQuery : IRequest<GetQueryResponse<Student>>
    {
        public string studentEmail = string.Empty;
    }
}
