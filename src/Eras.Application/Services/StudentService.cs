using Eras.Domain.Entities;
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
        public Student CreateStudent(Student student)
        {
            Console.WriteLine("------ Creando student ------");
            Console.WriteLine(student.Email);
            Console.WriteLine(student.Name);
            Console.WriteLine(student.CreatedDate);
            /* 
            Aqui deberia unirme con manuel..
            Llamar a interfaz de persistencia, 
                buscar por email,si existe retornar student
                sino crear nuevo
                {
                    ValidateNewStudent(student); // verificar logica de negocio, email no repetido, tamaño etc
                    GuardarStudent..
                    Retornar student guardado
                }
            */
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
