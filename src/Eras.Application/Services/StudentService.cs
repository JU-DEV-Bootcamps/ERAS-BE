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
            try
            {
                return await  _studentRepository.Add(student);
            }
            catch (Exception e)
            {
                throw new NotImplementedException("Error creating student: " + e.Message);
            }
        }

        public void ValidateNewStudent(Student student)
        {
            throw new NotImplementedException();
        }
    }
}
