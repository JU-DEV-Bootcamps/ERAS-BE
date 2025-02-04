using Eras.Application.Contracts;
using Eras.Application.DTOs;
using Eras.Application.Mappers;
using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<StudentService> _logger;

        public StudentService(IStudentRepository studentRepository, ILogger<StudentService> logger)
        {
            _studentRepository = studentRepository;
            _logger = logger;
        }

        public async Task<Student> CreateStudent(Student student)
        {
            return await _studentRepository.AddAsync(student);
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

                    var student = dto.ToDomain();

                    await _studentRepository.AddAsync(student);
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
            return !string.IsNullOrWhiteSpace(dto.Name) 
                && !string.IsNullOrWhiteSpace(dto.Email) 
                && !string.IsNullOrWhiteSpace(dto.SISId);
        }
    }
}
