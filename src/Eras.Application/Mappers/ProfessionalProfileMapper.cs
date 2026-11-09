using Eras.Application.DTOs.UsersManagement;
using Eras.Domain.Entities.UserManagement;

namespace Eras.Application.Mappers;

public static class ProfessionalProfileMapper
{
    public static ProfessionalProfileDTO ToDTO(this ProfessionalProfile Profile, string? Position)
    {
        ArgumentNullException.ThrowIfNull(Profile);
        return new ProfessionalProfileDTO
        {
            Position = Position ?? string.Empty,
            Faculty = Profile.Faculty,
            OfficeHours = Profile.OfficeHours,
            SubjectsTaught = Profile.SubjectsTaught,
            Courses = Profile.Courses
                .Select(Course => new ProfessionalCourseDTO(Course.Code, Course.Name))
                .ToArray(),
        };
    }
}
