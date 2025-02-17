using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class StudentCohortMapper
    {
        public static Student ToDomain(this StudentCohortJoin studentCohortJoin)
        {
            ArgumentNullException.ThrowIfNull(studentCohortJoin);
            return new Student()
            {
                CohortId = studentCohortJoin.CohortId,
            };
        }
        public static StudentCohortJoin ToPersistenceCohort(this Student student)
        {
            ArgumentNullException.ThrowIfNull(student);
            return new StudentCohortJoin()
            {
                StudentId = student.Id,
                CohortId = student.CohortId,
            };
        }
    }
}
