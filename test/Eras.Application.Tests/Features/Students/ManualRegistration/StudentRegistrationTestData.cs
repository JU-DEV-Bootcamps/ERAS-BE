using Eras.Application.DTOs.Student;

namespace Eras.Application.Tests.Features.Students.ManualRegistration;

internal static class StudentRegistrationTestData
{
    public static StudentRegistrationDto ValidDto() => new()
    {
        CohortId = 3,
        FirstName = "Ana",
        MiddleName = "María",
        LastName = "Pérez",
        IdPassportNumber = "ab-123456",
        PrimaryEmail = "ana.perez@jala.university",
    };
}
