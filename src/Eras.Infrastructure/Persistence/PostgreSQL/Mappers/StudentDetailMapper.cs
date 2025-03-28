using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class StudentDetailMapper
    {
        public static StudentDetail ToDomain(this StudentDetailEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            return new StudentDetail
            {
                Id = entity.Id,
                StudentId = entity.StudentId,
                EnrolledCourses = entity.EnrolledCourses,
                GradedCourses = entity.GradedCourses,
                TimeDeliveryRate = entity.TimeDeliveryRate,
                AvgScore = entity.AvgScore,
                CoursesUnderAvg = entity.CoursesUnderAvg,
                PureScoreDiff = entity.PureScoreDiff,
                StandardScoreDiff = entity.StandardScoreDiff,
                LastAccessDays = entity.LastAccessDays,
                Audit = entity.Audit
            };
        }

        public static StudentDetailEntity ToPersistence(this StudentDetail model)
        {
            ArgumentNullException.ThrowIfNull(model);
            return new StudentDetailEntity
            {
                Id = model.Id,
                StudentId = model.StudentId,
                EnrolledCourses = model.EnrolledCourses,
                GradedCourses = model.GradedCourses,
                TimeDeliveryRate = model.TimeDeliveryRate,
                AvgScore = model.AvgScore,
                CoursesUnderAvg = model.CoursesUnderAvg,
                PureScoreDiff = model.PureScoreDiff,
                StandardScoreDiff = model.StandardScoreDiff,
                LastAccessDays = model.LastAccessDays,
                Audit = model.Audit
            };
        }
    }
}