using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Eras.Infrastructure.Persistence.Mappers;
using Eras.Infrastructure.Persistence.PostgreSQL;

namespace Eras.Infrastructure.Persistence.Repositories
{
    public class StudentRepository : IStudentRepository<Student>
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Student student)
        {
            var studentEntity = student.ToStudentEntity();
            _context.Students.Add(studentEntity);
            await _context.SaveChangesAsync();
        }
    }
}
