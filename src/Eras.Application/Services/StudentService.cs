using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Services
{
    public class StudentService(IStudentRepository StudentRepository, ILogger<StudentService> Logger) : IStudentService
    {
        private readonly IStudentRepository _studentRepository = StudentRepository;

        public async Task<Student> CreateStudent(Student Student)
        {
            try
            {
                return await _studentRepository.AddAsync(Student);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while creating the student");
                throw new Exception("An error occurred while creating the student", ex);
            }
        }

        public Task<int> ImportStudentsAsync(StudentImportDto[] StudentsDto)
        {
            var newRecords = 0;
            foreach (StudentImportDto dto in StudentsDto)
            {
                try
                {
                    if (!ValidateStudentDto(dto))
                    {
                        continue;
                    }
                    var created = new Student();
                    if (created != null) newRecords++;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "An error occurred during the import process");
                }
                return Task.FromResult(newRecords);
            }
            return Task.FromResult(newRecords);
        }

        private static bool ValidateStudentDto(StudentImportDto Dto) => !string.IsNullOrWhiteSpace(Dto.Name)
                && !string.IsNullOrWhiteSpace(Dto.Email)
                && !string.IsNullOrWhiteSpace(Dto.SISId);
    }
}
