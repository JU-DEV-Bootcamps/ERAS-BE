using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.DTOs;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Events
{
    public class CreateStudentsEvent : IRequest<bool>
    {
        public Student student;
        public CreateStudentsEvent(Student student) {
            this.student = student;
        }
    }
}
