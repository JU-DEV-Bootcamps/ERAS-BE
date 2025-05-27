using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class StudentCohortMapper
    {
        public static Student ToDomain(this StudentCohortJoin StudentCohortJoin)
        {
            ArgumentNullException.ThrowIfNull(StudentCohortJoin);
            return new Student()
            {
                Uuid = StudentCohortJoin.Student.Uuid,
                Id = StudentCohortJoin.Student.Id,
                Name = StudentCohortJoin.Student.Name,
                Email = StudentCohortJoin.Student.Email,
                CohortId = StudentCohortJoin.CohortId,
            };
        }

        public static Student ToJoinDomain(this StudentCohortJoin StudentCohortJoin)
        {
            ArgumentNullException.ThrowIfNull(StudentCohortJoin);
            return new Student()
            {
                Uuid = StudentCohortJoin.Student.Uuid,
                Id = StudentCohortJoin.Student.Id,
                Name = StudentCohortJoin.Student.Name,
                Email = StudentCohortJoin.Student.Email,
                Cohort = StudentCohortJoin.Cohort.ToDomain(),
            };

        }


        public static StudentCohortJoin ToPersistenceCohort(this Student Student)
        {
            ArgumentNullException.ThrowIfNull(Student);
            return new StudentCohortJoin()
            {
                StudentId = Student.Id,
                CohortId = Student.CohortId,
            };
        }
    }
}
