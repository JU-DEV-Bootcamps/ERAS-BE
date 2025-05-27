using Eras.Application.DTOs;
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class StudentDetailMapper
    {
        public static StudentDetail ToDomain(this StudentDetailDTO Dto)
        {
            ArgumentNullException.ThrowIfNull(Dto);

            AuditInfo audit = Dto.Audit != null ? Dto.Audit : new AuditInfo()
            {
                CreatedBy = "Automatic mapper",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
            };
            return new StudentDetail
            {
                StudentId = Dto.StudentId,
                EnrolledCourses = Dto.EnrolledCourses,
                GradedCourses = Dto.GradedCourses,
                TimeDeliveryRate = Dto.TimeDeliveryRate,
                AvgScore = Dto.AvgScore,
                CoursesUnderAvg= Dto.CoursesUnderAvg,
                PureScoreDiff = Dto.PureScoreDiff,
                StandardScoreDiff = Dto.StandardScoreDiff,
                LastAccessDays = Dto.LastAccessDays,
                Audit = audit,
            };

        }
        public static StudentDetailDTO ToDto(this StudentDetail Domain)
        {
            ArgumentNullException.ThrowIfNull(Domain);
            return new StudentDetailDTO
            {
                StudentId = Domain.StudentId,
                EnrolledCourses = Domain.EnrolledCourses,
                GradedCourses = Domain.GradedCourses,
                TimeDeliveryRate = Domain.TimeDeliveryRate,
                AvgScore = Domain.AvgScore,
                CoursesUnderAvg = Domain.CoursesUnderAvg,
                PureScoreDiff = Domain.PureScoreDiff,
                StandardScoreDiff = Domain.StandardScoreDiff,
                LastAccessDays = Domain.LastAccessDays,
                Audit = Domain.Audit,
            };

        }
    }
}
