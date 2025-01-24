using Eras.Domain.Entities;

namespace Eras.Domain.Services
{
    public interface IStudentService
    {
        public Task<Student> CreateStudent(Student student);
    }
}
