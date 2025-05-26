using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<StudentService> _logger;


        public StudentService(IStudentRepository StudentRepository, ILogger<StudentService> Logger)
        {
            _studentRepository = StudentRepository;
            _logger = Logger;
        }

        public async Task<Student?> CreateStudent(Student Student)
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
                return await _studentRepository.AddAsync(Student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred during the creation of student {Student.Id}", ex.Message);
                return null;
            }
        }

        public async Task<int> ImportStudentsAsync(StudentImportDto[] StudentsDto)
        {
            int newRecords = 0;
            foreach (var dto in StudentsDto)
            {
                try
                {
                    if (!ValidateStudentDto(dto))
                    {
                        continue;
                    }
                    Student created = new Student();//await CreateStudent(dto.ToDomain());
                    if (created != null) newRecords++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred during the import process {ex.Message}");
                }

                return newRecords;
            }
            return newRecords;
        }

        private bool ValidateStudentDto(StudentImportDto Dto)
        {
            return !string.IsNullOrWhiteSpace(Dto.Name) 
                && !string.IsNullOrWhiteSpace(Dto.Email) 
                && !string.IsNullOrWhiteSpace(Dto.SISId);
        }
    }
}
