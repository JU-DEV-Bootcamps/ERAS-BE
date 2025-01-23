using Eras.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Domain.Services
{
    public interface IStudentService
    {
        public Student CreateStudent(Student student);
    }
}
