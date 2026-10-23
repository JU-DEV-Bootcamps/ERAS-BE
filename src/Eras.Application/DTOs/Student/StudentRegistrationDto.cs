namespace Eras.Application.DTOs.Student;

/// <summary>
/// Request/response shape for the "New Student" form. <see cref="PrimaryEmail"/> maps to
/// the student's login/contact email (<c>students.email</c>); everything else lives in
/// the 1:1 student profile.
/// </summary>
public sealed class StudentRegistrationDto
{
    public int? StudentId { get; set; }
    public int? CohortId { get; set; }
    public bool IsImported { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Nationality { get; set; }
    public string? CountryOfBirth { get; set; }
    public string IdPassportNumber { get; set; } = string.Empty;

    public string PrimaryEmail { get; set; } = string.Empty;
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
}
