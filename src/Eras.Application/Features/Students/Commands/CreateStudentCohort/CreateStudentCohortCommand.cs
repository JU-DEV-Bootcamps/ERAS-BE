using Eras.Application.DTOs;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.Students.Commands.CreateStudentCohort
{
    public class CreateStudentCohortCommand : IRequest<CreateCommandResponse<Student>>
    {
        public int StudentId;
        public int CohortId;
    }
}
