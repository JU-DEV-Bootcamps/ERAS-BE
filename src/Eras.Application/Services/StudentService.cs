using Eras.Application.DTOs;
using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Eras.Application.Services
{
    public class StudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<StudentService> _logger;

        public StudentService(IStudentRepository studentRepository, ILogger<StudentService> logger)
        {
            _studentRepository = studentRepository;
            _logger = logger;
        }

        public async Task<bool> ImportStudentsAsync(StudentImportDto[] studentsDto)
        {
            try
            {
                foreach (var dto in studentsDto)
                {
                    if (!ValidateStudentDto(dto))
                    {
                        _logger.LogWarning("Invalid student data: {SISId}", dto.SISId);
                        continue;
                    }

                    var student = MapToDomain(dto);

                    await _studentRepository.SaveAsync(student);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the import process");
                return false;
            }
        }

        private bool ValidateStudentDto(StudentImportDto dto)
        {
            return !string.IsNullOrWhiteSpace(dto.Name) &&
                   !string.IsNullOrWhiteSpace(dto.Email) &&
                   !string.IsNullOrWhiteSpace(dto.SISId);
        }

        private Student MapToDomain(StudentImportDto dto)
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

        private decimal ParseDecimal(decimal value, CultureInfo culture)
        {
            return decimal.TryParse(value.ToString(culture), NumberStyles.Number, culture, out var result) ? result : 0;
        }
    }
}
