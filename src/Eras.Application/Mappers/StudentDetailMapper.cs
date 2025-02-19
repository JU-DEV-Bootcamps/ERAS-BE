using Eras.Application.DTOs;
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class StudentDetailMapper
    {
        public static StudentDetail ToDomain(this StudentDetailDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            AuditInfo audit = dto.Audit != null ? dto.Audit : new AuditInfo()
            {
                CreatedBy = "Automatic mapper",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
            };
            return new StudentDetail
            {
                StudentId = dto.StudentId,
                EnrolledCourses = dto.EnrolledCourses,
                GradedCourses = dto.GradedCourses,
                TimeDeliveryRate = dto.TimeDeliveryRate,
                AvgScore = dto.AvgScore,
                CoursesUnderAvg= dto.CoursesUnderAvg,
                PureScoreDiff = dto.PureScoreDiff,
                StandardScoreDiff = dto.StandardScoreDiff,
                LastAccessDays = dto.LastAccessDays,
                Audit = audit,
            };

        }
        public static StudentDetailDTO ToDto(this StudentDetail domain)
        {
            ArgumentNullException.ThrowIfNull(domain);
            return new StudentDetailDTO
            {
                StudentId = domain.StudentId,
                EnrolledCourses = domain.EnrolledCourses,
                GradedCourses = domain.GradedCourses,
                TimeDeliveryRate = domain.TimeDeliveryRate,
                AvgScore = domain.AvgScore,
                CoursesUnderAvg = domain.CoursesUnderAvg,
                PureScoreDiff = domain.PureScoreDiff,
                StandardScoreDiff = domain.StandardScoreDiff,
                LastAccessDays = domain.LastAccessDays,
                Audit = domain.Audit,
            };

        }
    }
}
