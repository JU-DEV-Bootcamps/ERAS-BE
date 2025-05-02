using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class StudentDetailMapper
    {
        public static StudentDetail ToDomain(this StudentDetailEntity Entity)
        {
            ArgumentNullException.ThrowIfNull(Entity);
            return new StudentDetail
            {
                Id = Entity.Id,
                StudentId = Entity.StudentId,
                EnrolledCourses = Entity.EnrolledCourses,
                GradedCourses = Entity.GradedCourses,
                TimeDeliveryRate = Entity.TimeDeliveryRate,
                AvgScore = Entity.AvgScore,
                CoursesUnderAvg = Entity.CoursesUnderAvg,
                PureScoreDiff = Entity.PureScoreDiff,
                StandardScoreDiff = Entity.StandardScoreDiff,
                LastAccessDays = Entity.LastAccessDays,
                Audit = Entity.Audit
            };
        }

        public static StudentDetailEntity ToPersistence(this StudentDetail Model)
        {
            ArgumentNullException.ThrowIfNull(Model);
            return new StudentDetailEntity
            {
                Id = Model.Id,
                StudentId = Model.StudentId,
                EnrolledCourses = Model.EnrolledCourses,
                GradedCourses = Model.GradedCourses,
                TimeDeliveryRate = Model.TimeDeliveryRate,
                AvgScore = Model.AvgScore,
                CoursesUnderAvg = Model.CoursesUnderAvg,
                PureScoreDiff = Model.PureScoreDiff,
                StandardScoreDiff = Model.StandardScoreDiff,
                LastAccessDays = Model.LastAccessDays,
                Audit = Model.Audit
            };
        }
    }
}
