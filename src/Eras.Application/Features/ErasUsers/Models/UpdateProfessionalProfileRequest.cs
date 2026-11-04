using Eras.Application.DTOs.UsersManagement;

namespace Eras.Application.Features.ErasUsers.Models;

/// <summary>
/// PUT body of the professional profile. Absent lists mean "no subjects" / "no courses".
/// </summary>
public sealed class UpdateProfessionalProfileRequest
{
    public string? Position { get; set; }
    public string? Faculty { get; set; }
    public string? OfficeHours { get; set; }
    public string[]? SubjectsTaught { get; set; }
    public ProfessionalCourseDTO[]? Courses { get; set; }
}
