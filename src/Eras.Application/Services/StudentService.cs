using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Eras.Domain.Services;

namespace Eras.Application.Services
{
    public class StudentService : IStudentService
    {

        private readonly IStudentRepository<Student> _studentRepository;
        public StudentService(IStudentRepository<Student> studentRepository)
        {
            _studentRepository = studentRepository;
        }
        public async Task<Student> CreateStudent(Student student)
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
            try
            {
                await  _studentRepository.Add(student);
            } catch (Exception e)
            {

            }
           


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
