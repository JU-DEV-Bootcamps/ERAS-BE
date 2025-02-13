using Eras.Application.Dtos;
using Eras.Application.DTOs; 
using Eras.Domain.Entities; 
using System.Globalization;

namespace Eras.Application.Mappers;

public static class StudentMapper
{
    public static Student ToDomain(this StudentDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new Student
        {
            Id = dto.Id,
            Uuid = dto.Uuid,
            Name = dto.Name,
            Email = dto.Email,
            StudentDetail = dto.StudentDetail?.ToDomain(),
            Audit = default
        };

    }
    public static StudentDTO ToDto(this Student domain)
    {
        ArgumentNullException.ThrowIfNull(domain);
        return new StudentDTO
        {
            Id = domain.Id,
            Uuid = domain.Uuid,
            Name = domain.Name,
            Email = domain.Email,
            StudentDetail = domain.StudentDetail?.ToDto(),
        };

    }
    public static Student ToDomain(this StudentImportDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var culture = CultureInfo.GetCultureInfo("es-ES");

        return new Student
        {
            Uuid = dto.SISId,
            Name = dto.Name,
            Email = dto.Email,
            StudentDetail = new StudentDetail
            {
                EnrolledCourses = dto.EnrolledCourses,
                GradedCourses = dto.GradedCourses,
                TimeDeliveryRate = dto.TimelySubmissions,
                AvgScore = ParseDecimal(dto.AverageScore, culture),
                CoursesUnderAvg = dto.CoursesBelowAverage,
                PureScoreDiff = ParseDecimal(dto.RawScoreDifference, culture),
                StandardScoreDiff = ParseDecimal(dto.StandardScoreDifference, culture),
                LastAccessDays = dto.DaysSinceLastAccess,
                Audit = new Domain.Common.AuditInfo
                {
                    CreatedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now,
                }
            },
            Audit = new Domain.Common.AuditInfo
            {
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
            }
        };
    }

    private static decimal ParseDecimal(decimal value, CultureInfo culture)
    {
        return decimal.TryParse(
            value.ToString(culture),
            NumberStyles.Number,
            culture, out var result
        ) ? result : 0;
    }
}
