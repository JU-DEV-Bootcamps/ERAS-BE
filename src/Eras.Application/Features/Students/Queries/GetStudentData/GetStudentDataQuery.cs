using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Students.Queries.GetStudentData
{
    public class GetStudentDataQuery : IRequest<CreateCommandResponse<Student>>
    {
    }
}
