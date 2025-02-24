using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class StudentCohortRepository : BaseRepository<Student, StudentCohortJoin>, IStudentCohortRepository
    {
        public StudentCohortRepository(AppDbContext context)
            : base(context, StudentCohortMapper.ToDomain, StudentCohortMapper.ToPersistenceCohort)
        {
        }

        public async Task<Student?> GetByCohortIdAndStudentIdAsync(int cohortId, int studentId)
        {
            var results = await _context.StudentCohorts.FirstOrDefaultAsync(studentCohort => studentCohort.StudentId.Equals(studentId) && studentCohort.CohortId.Equals(cohortId));
            return results?.ToDomain();
        }

        public async Task<IEnumerable<Student>?> GetAllStudentsByCohortIdAsync(int cohortId)
        {
            var cohortStudents = await _context.StudentCohorts.Include(cs => cs.Student).Where(studentCohort => studentCohort.CohortId.Equals(cohortId)).ToListAsync();
            var domainStudents = new List<Student>();
            foreach(var student in cohortStudents)
            {
                domainStudents.Add(student.ToDomain());
            }
            return domainStudents;
        }
    }
}
