using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class StudentRepository : BaseRepository<Student, StudentEntity>, IStudentRepository
    {
        public StudentRepository(AppDbContext context)
            : base(context, StudentMapper.ToDomain, StudentMapper.ToPersistence) { }

        public async Task<Student?> GetByNameAsync(string name)
        {
            var student = await _context.Students.FirstOrDefaultAsync(student =>
                student.Name == name
            );

            return student?.ToDomain();
        }

        public async Task<Student?> GetByUuidAsync(string uuid)
        {
            var student = await _context.Students.FirstOrDefaultAsync(student =>
                student.Uuid == uuid
            );

            return student?.ToDomain();
        }

        public async Task<Student?> GetByEmailAsync(string email)
        {
            var student = await _context.Students.FirstOrDefaultAsync(student =>
                student.Email == email
            );

            return student?.ToDomain();
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return (await _context.Students.ToListAsync())
                .Select(student => student.ToDomain())
                .ToList();
        }
    }
}
