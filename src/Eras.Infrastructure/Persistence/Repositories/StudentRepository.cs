using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Eras.Infrastructure.Persistence.Mappers;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.Repositories
{
    public class StudentRepository : IStudentRepository<Student>
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public StudentRepository(IDbContextFactory<AppDbContext> context)
        {
            _contextFactory = context;
        }

        public async Task Add(Student student)
        {
            var context = _contextFactory.CreateDbContext();
            var studentEntity = student.ToStudentEntity();
            context.Students.Add(studentEntity);
            await context.SaveChangesAsync();
        }
    }
}
