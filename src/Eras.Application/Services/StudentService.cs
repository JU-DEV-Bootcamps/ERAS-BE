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
                    // Attempt to parse decimal fields with commas
                    if (!decimal.TryParse(dto.PuntuacionMedia, NumberStyles.Number, culture, out var avgScore))
                        avgScore = 0;

                    if (!decimal.TryParse(dto.DiferenciaDeLaPuntuacionPura, NumberStyles.Number, culture, out var pureScoreDiff))
                        pureScoreDiff = 0;

                    if (!decimal.TryParse(dto.DiferenciaDeLaPuntuacionEstandarizada, NumberStyles.Number, culture, out var standardScoreDiff))
                        standardScoreDiff = 0;

                    // Check if student already exists in domain
                    var existingStudent = await _studentRepository
                        .GetByUuidAsync(dto.IdentificacionDeSISDelUsuario);

                    if (existingStudent == null)
                    {
                        // Create new domain entity
                        existingStudent = new Student
                        {
                            Uuid = dto.IdentificacionDeSISDelUsuario,
                            Name = dto.Nombre,
                            Email = dto.CorreoElectronico,
                            StudentDetail = new StudentDetail
                            {
                                EnrolledCourses = dto.CursosInscritos,
                                GradedCourses = dto.CursosConNota,
                                TimeDeliveryRate = dto.EntregasATiempoEnComparacionConTodas,
                                AvgScore = avgScore,
                                CoursesUnderAvg = dto.CursosConUnaNotaMediaPorDebajoDe,
                                PureScoreDiff = pureScoreDiff,
                                StandardScoreDiff = standardScoreDiff,
                                LastAccessDays = dto.DiasDesdeElUltimoAcceso
                            }
                        };
                    }
                    else
                    {
                        // Update domain entity
                        existingStudent.Name = dto.Nombre;
                        existingStudent.Email = dto.CorreoElectronico;

                        if (existingStudent.StudentDetail == null)
                            existingStudent.StudentDetail = new StudentDetail();

                        existingStudent.StudentDetail.EnrolledCourses = dto.CursosInscritos;
                        existingStudent.StudentDetail.GradedCourses = dto.CursosConNota;
                        existingStudent.StudentDetail.TimeDeliveryRate = dto.EntregasATiempoEnComparacionConTodas;
                        existingStudent.StudentDetail.AvgScore = avgScore;
                        existingStudent.StudentDetail.CoursesUnderAvg = dto.CursosConUnaNotaMediaPorDebajoDe;
                        existingStudent.StudentDetail.PureScoreDiff = pureScoreDiff;
                        existingStudent.StudentDetail.StandardScoreDiff = standardScoreDiff;
                        existingStudent.StudentDetail.LastAccessDays = dto.DiasDesdeElUltimoAcceso;
                    }

                    // Now save
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
