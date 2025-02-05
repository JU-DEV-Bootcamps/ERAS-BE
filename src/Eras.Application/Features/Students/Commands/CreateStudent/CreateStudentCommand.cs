using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentCommand : IRequest<bool>
    {
        public Student student;
        public CreateStudentCommand(Student student)
        {
            this.student = student;
        }
        
    }
}
