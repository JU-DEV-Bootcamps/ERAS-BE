using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Dtos;
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
