using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Mappers;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;


        public StudentService(IStudentRepository studentRepository, ILogger<StudentService> logger)
        {
            _studentRepository = studentRepository;

        }

        public async Task<Student> CreateStudent(Student student)
        {
            try
            {
                /*
                if (!ValidateStudentDto(dto))
                {
                    _logger.LogWarning("Invalid student data: {SISId}", dto.SISId);
                    continue;
                }
                */
                return await _studentRepository.AddAsync(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the student creation");
                return null;
            }
        }

        public async Task<int> ImportStudentsAsync(StudentImportDto[] studentsDto)
        {
            int newRecors = 0;
            foreach (var dto in studentsDto)
            {
                try
                {
                    if (!ValidateStudentDto(dto))
                    {
                        continue;
                    }
                    Student created = await CreateStudent(dto.ToDomain());
                    if (created != null) newRecors++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during the import process");
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            return newRecors;
        }

        private bool ValidateStudentDto(StudentImportDto dto)
        {
            return !string.IsNullOrWhiteSpace(dto.Name) 
                && !string.IsNullOrWhiteSpace(dto.Email) 
                && !string.IsNullOrWhiteSpace(dto.SISId);
        }
    }
}
