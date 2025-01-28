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
                var culture = CultureInfo.GetCultureInfo("es-ES");

                foreach (var dto in studentsDto)
                {
                    if (!decimal.TryParse(dto.AverageScore.ToString(), NumberStyles.Number, culture, out var avgScore))
                        avgScore = 0;

                    if (!decimal.TryParse(dto.RawScoreDifference.ToString(), NumberStyles.Number, culture, out var pureScoreDiff))
                        pureScoreDiff = 0;

                    if (!decimal.TryParse(dto.StandardScoreDifference.ToString(), NumberStyles.Number, culture, out var standardScoreDiff))
                        standardScoreDiff = 0;

                    var existingStudent = await _studentRepository
                        .GetByUuidAsync(dto.SISId);

                    if (existingStudent == null)
                    {
                        existingStudent = new Student
                        {
                            Uuid = dto.SISId,
                            Name = dto.Name,
                            Email = dto.Email,
                            StudentDetail = new StudentDetail
                            {
                                EnrolledCourses = dto.EnrolledCourses,
                                GradedCourses = dto.GradedCourses,
                                TimeDeliveryRate = dto.TimelySubmissions,
                                AvgScore = avgScore,
                                CoursesUnderAvg = dto.CoursesBelowAverage,
                                PureScoreDiff = pureScoreDiff,
                                StandardScoreDiff = standardScoreDiff,
                                LastAccessDays = dto.DaysSinceLastAccess
                            }
                        };
                    }
                    else
                    {
                        existingStudent.Name = dto.Name;
                        existingStudent.Email = dto.Email;

                        if (existingStudent.StudentDetail == null)
                            existingStudent.StudentDetail = new StudentDetail();

                        existingStudent.StudentDetail.EnrolledCourses = dto.EnrolledCourses;
                        existingStudent.StudentDetail.GradedCourses = dto.GradedCourses;
                        existingStudent.StudentDetail.TimeDeliveryRate = dto.TimelySubmissions;
                        existingStudent.StudentDetail.AvgScore = avgScore;
                        existingStudent.StudentDetail.CoursesUnderAvg = dto.CoursesBelowAverage;
                        existingStudent.StudentDetail.PureScoreDiff = pureScoreDiff;
                        existingStudent.StudentDetail.StandardScoreDiff = standardScoreDiff;
                        existingStudent.StudentDetail.LastAccessDays = dto.DaysSinceLastAccess;
                    }
                    await _studentRepository.SaveAsync(existingStudent);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the import process");
                return false;
            }
        }
    }
}
