
using Eras.Application.DTOs.RemissionManagement;
using Eras.Domain.Entities.RemissionsManagement;

namespace Eras.Application.Mappers.RemissionManagement;

public sealed class StudentProfileToDtoMapper : IMapper<StudentProfile, StudentProfileDto>
{
    public StudentProfileDto Map(StudentProfile source)
    {
        return new StudentProfileDto
        {
            Id = source.Id,
            StudentCode = source.StudentCode,
            FirstName = source.FirstName,
            LastName = source.LastName,
            SupportAndReferralHistory = source.SupportAndReferralHistory,
            CharacterizationOrCurrentContext = source.CharacterizationOrCurrentContext
        };
    }
}