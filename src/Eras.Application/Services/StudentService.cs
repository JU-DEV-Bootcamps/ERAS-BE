using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Eras.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        public Student CreateStudent(Student student)
        {
            Console.WriteLine("------ Creando student ------");
            Console.WriteLine(student.Email);
            Console.WriteLine(student.Name);
            Console.WriteLine(student.CreatedDate);
      
            ///agreagar bd
            _studentRepository.Add(student);
            return student;
        }

        public void ValidateNewStudent(Student student)
        {
            Console.WriteLine("Validar que email esta correcto");
            Console.WriteLine("Validando que nombre es correcto");
            throw new NotImplementedException();
        }
    }
}
