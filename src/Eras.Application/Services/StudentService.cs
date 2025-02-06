using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
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
                        continue;
                    }

                    var student = dto.ToDomain();

                    await _studentRepository.AddAsync(student);
                }

                return true;
            }
            catch (Exception ex)
            {
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
