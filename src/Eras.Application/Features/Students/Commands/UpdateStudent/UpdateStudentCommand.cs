using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Students.Commands.UpdateStudent
{
    public class UpdateStudentCommand : IRequest<CreateCommandResponse<Student>>
    {
        public StudentDTO? StudentDTO;
    }
}
