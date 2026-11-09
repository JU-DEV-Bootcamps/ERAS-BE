namespace Eras.Application.DTOs.UsersManagement;

/// <summary>
/// The professional profile as returned to its owner. Position is read from the user record.
/// </summary>
public sealed class ProfessionalProfileDTO
{
    public string Position { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public string OfficeHours { get; set; } = string.Empty;
    public string[] SubjectsTaught { get; set; } = [];
    public ProfessionalCourseDTO[] Courses { get; set; } = [];
}

public sealed record ProfessionalCourseDTO(string Code, string Name);
