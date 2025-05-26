using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.StudentsDetails.Commands.CreateStudentDetail
{
    public class CreateStudentDetailCommand : IRequest<CreateCommandResponse<StudentDetail>>
    {
        public StudentDetailDTO? StudentDetailDto;
    }
}
