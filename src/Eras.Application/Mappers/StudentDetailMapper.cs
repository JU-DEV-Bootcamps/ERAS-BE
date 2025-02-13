using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers
{
    public static class StudentDetailMapper
    {
        public static StudentDetail ToDomain(this StudentDetailDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            return new StudentDetail
            {
                EnrolledCourses = dto.EnrolledCourses,
                GradedCourses = dto.GradedCourses,
                TimeDeliveryRate = dto.TimeDeliveryRate,
                AvgScore = dto.AvgScore,
                CoursesUnderAvg= dto.CoursesUnderAvg,
                PureScoreDiff = dto.PureScoreDiff,
                StandardScoreDiff = dto.StandardScoreDiff,
                LastAccessDays = dto.LastAccessDays,
                Audit = dto.Audit,
            };

        }
        public static StudentDetailDTO ToDto(this StudentDetail domain)
        {
            ArgumentNullException.ThrowIfNull(domain);
            return new StudentDetailDTO
            {
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
