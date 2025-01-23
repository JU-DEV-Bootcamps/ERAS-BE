using Eras.Domain.Repositories;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Repositories
{
    public class StudentRepository<Students> : IStudentRepository<Students>
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Students student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));
            _context.Students.Add(student);
            _context.SaveChanges(); 
        }
    }
}