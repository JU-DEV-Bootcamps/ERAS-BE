using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Mappers.AssessmentManagement;

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