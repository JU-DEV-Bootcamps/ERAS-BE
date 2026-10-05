using Eras.Application.DTOs.Student;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers;

public static class StudentRegistrationMapper
{
    /// <summary>
    /// Display name stored in <c>students.name</c>: first, middle and last name joined.
    /// </summary>
    public static string ComposeFullName(this StudentRegistrationDto Dto)
    {
        return string.Join(' ', new[] { Dto.FirstName, Dto.MiddleName, Dto.LastName }
            .Where(Part => !string.IsNullOrWhiteSpace(Part))
            .Select(Part => Part!.Trim()));
    }

    public static void ApplyTo(this StudentRegistrationDto Dto, StudentProfile Profile)
    {
        Profile.FirstName = Dto.FirstName.Trim();
        Profile.MiddleName = Clean(Dto.MiddleName);
        Profile.LastName = Dto.LastName.Trim();
        Profile.DateOfBirth = Dto.DateOfBirth;
        Profile.Gender = Clean(Dto.Gender);
        Profile.Nationality = Clean(Dto.Nationality);
        Profile.CountryOfBirth = Clean(Dto.CountryOfBirth);
        Profile.IdPassportNumber = NormalizeIdPassport(Dto.IdPassportNumber);
        Profile.SecondaryEmail = Clean(Dto.SecondaryEmail);
        Profile.MobileNumber = Clean(Dto.MobileNumber);
        Profile.Country = Clean(Dto.Country);
        Profile.StateProvince = Clean(Dto.StateProvince);
        Profile.City = Clean(Dto.City);
        Profile.Street = Clean(Dto.Street);
        Profile.PostalCode = Clean(Dto.PostalCode);
        Profile.LevelOfStudy = Clean(Dto.LevelOfStudy);
        Profile.FacultySchool = Clean(Dto.FacultySchool);
        Profile.StudyModality = Clean(Dto.StudyModality);
        Profile.PreviousInstitution = Clean(Dto.PreviousInstitution);
    }

    public static StudentRegistrationDto ToDto(this StudentProfile Profile, Student Student)
    {
        return new StudentRegistrationDto
        {
            StudentId = Student.Id,
            IsImported = Student.IsImported,
            FirstName = Profile.FirstName,
            MiddleName = Profile.MiddleName,
            LastName = Profile.LastName,
            DateOfBirth = Profile.DateOfBirth,
            Gender = Profile.Gender,
            Nationality = Profile.Nationality,
            CountryOfBirth = Profile.CountryOfBirth,
            IdPassportNumber = Profile.IdPassportNumber,
            PrimaryEmail = Student.Email,
            SecondaryEmail = Profile.SecondaryEmail,
            MobileNumber = Profile.MobileNumber,
            Country = Profile.Country,
            StateProvince = Profile.StateProvince,
            City = Profile.City,
            Street = Profile.Street,
            PostalCode = Profile.PostalCode,
            LevelOfStudy = Profile.LevelOfStudy,
            FacultySchool = Profile.FacultySchool,
            StudyModality = Profile.StudyModality,
            PreviousInstitution = Profile.PreviousInstitution,
        };
    }

    public static string NormalizeIdPassport(string Value) => Value.Trim().ToUpperInvariant();

    private static string? Clean(string? Value) =>
        string.IsNullOrWhiteSpace(Value) ? null : Value.Trim();
}
