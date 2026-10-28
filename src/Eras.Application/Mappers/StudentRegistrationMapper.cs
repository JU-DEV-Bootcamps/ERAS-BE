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
        Profile.SisId = Clean(Dto.SisId);
        Profile.Cgpa = Dto.Cgpa;
        Profile.SapStatus = Clean(Dto.SapStatus);
        Profile.IsCurrentCourseThirdAttempt = Dto.IsCurrentCourseThirdAttempt;
        Profile.PassedCoursesCount = Dto.PassedCoursesCount;
        Profile.FailedCoursesCount = Dto.FailedCoursesCount;
        Profile.CurrentCourses = Dto.CurrentCourses?
            .Where(Course => !string.IsNullOrWhiteSpace(Course.Name))
            .Select(Course => new StudentCurrentCourse
            {
                Name = Course.Name.Trim(),
                Group = Clean(Course.Group),
                Section = Clean(Course.Section),
            })
            .ToList();
        Profile.IdentifiedRisks = CleanList(Dto.IdentifiedRisks);
        Profile.SupportReceived = CleanList(Dto.SupportReceived);
    }

    public static StudentRegistrationDto ToDto(this StudentProfile Profile, Student Student, int? CohortId = null)
    {
        return new StudentRegistrationDto
        {
            StudentId = Student.Id,
            CohortId = CohortId,
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
            SisId = Profile.SisId,
            Cgpa = Profile.Cgpa,
            SapStatus = Profile.SapStatus,
            IsCurrentCourseThirdAttempt = Profile.IsCurrentCourseThirdAttempt,
            PassedCoursesCount = Profile.PassedCoursesCount,
            FailedCoursesCount = Profile.FailedCoursesCount,
            CurrentCourses = Profile.CurrentCourses?
                .Select(Course => new StudentCurrentCourseDto
                {
                    Name = Course.Name,
                    Group = Course.Group,
                    Section = Course.Section,
                })
                .ToList(),
            IdentifiedRisks = Profile.IdentifiedRisks,
            SupportReceived = Profile.SupportReceived,
        };
    }

    public static string NormalizeIdPassport(string Value) => Value.Trim().ToUpperInvariant();

    private static string? Clean(string? Value) =>
        string.IsNullOrWhiteSpace(Value) ? null : Value.Trim();

    private static List<string>? CleanList(List<string>? Values) =>
        Values?.Where(Item => !string.IsNullOrWhiteSpace(Item)).Select(Item => Item.Trim()).ToList();
}
