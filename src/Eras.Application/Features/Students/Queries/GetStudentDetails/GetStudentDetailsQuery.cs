using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Students.Queries.GetStudentDetails
{
    public class GetStudentDetailsQuery : IRequest<CreateCommandResponse<Student>>
    {
        public int? StudentDetailId;
    }
}
