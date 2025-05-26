using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentCommand : IRequest<CreateCommandResponse<Student?>>
    {
        public required StudentDTO StudentDTO;
    }
}
