using Eras.Domain.Common;

namespace Eras.Domain.Entities;

/// <summary>
/// Personal, contact and academic information for a student, kept apart from
/// <see cref="Student"/> (1:1) so imported students and every import/export flow stay
/// untouched. Only students registered through the "New Student" flow (or later edited
/// there) have one.
/// </summary>
public class StudentProfile : BaseEntity, IAuditableEntity
{
    public int StudentId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Nationality { get; set; }
    public string? CountryOfBirth { get; set; }
    public string IdPassportNumber { get; set; } = string.Empty;

    public string? SecondaryEmail { get; set; }
    public string? MobileNumber { get; set; }
    public string? Country { get; set; }
    public string? StateProvince { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? PostalCode { get; set; }

    public string? LevelOfStudy { get; set; }
    public string? FacultySchool { get; set; }
    public string? StudyModality { get; set; }
    public string? PreviousInstitution { get; set; }

    public AuditInfo Audit { get; set; } = default!;
}
