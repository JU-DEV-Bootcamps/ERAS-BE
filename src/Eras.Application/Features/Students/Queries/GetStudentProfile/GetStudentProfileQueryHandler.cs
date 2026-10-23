using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Student;
using Eras.Application.Mappers;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using MediatR;

namespace Eras.Application.Features.Students.Queries.GetStudentProfile;

public sealed class GetStudentProfileQueryHandler(
    IStudentRepository StudentRepository,
    IStudentProfileRepository ProfileRepository
) : IRequestHandler<GetStudentProfileQuery, StudentRegistrationDto>
{
    public async Task<StudentRegistrationDto> Handle(
        GetStudentProfileQuery Request,
        CancellationToken CancellationToken)
    {
        Student student = await StudentRepository.GetByIdAsync(Request.StudentId)
            ?? throw new NotFoundException($"Student with ID {Request.StudentId} not found.");

        StudentProfile? profile = await ProfileRepository.GetByStudentIdAsync(student.Id);
        if (profile is not null) return profile.ToDto(student);

        string[] nameParts = student.Name.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return new StudentRegistrationDto
        {
            StudentId = student.Id,
            IsImported = student.IsImported,
            FirstName = nameParts.ElementAtOrDefault(0) ?? string.Empty,
            LastName = nameParts.ElementAtOrDefault(1) ?? string.Empty,
            PrimaryEmail = student.Email,
        };
    }
}
