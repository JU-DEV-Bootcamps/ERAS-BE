using Eras.Application.DTOs;
using Eras.Domain.Entities;
using System.Globalization;

namespace Eras.Application.Mappers;

public static class StudentMapper
{
    public static Student ToDomain(this StudentImportDto dto)
    {
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
                LastAccessDays = dto.DaysSinceLastAccess
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
