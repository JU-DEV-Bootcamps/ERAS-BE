using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.DTOs;
using Eras.Application.Models;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.StudentsDetails.Commands.CreateStudentDetail
{
    public class CreateStudentDetailCommand : IRequest<CreateCommandResponse<StudentDetail>>
    {
        public StudentDetailDTO? StudentDetailDto;
    }
}
