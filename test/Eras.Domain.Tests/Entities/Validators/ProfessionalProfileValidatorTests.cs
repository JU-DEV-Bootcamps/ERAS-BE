using Eras.Domain.Entities.UserManagement;

using FluentValidation.TestHelper;

using Xunit;

namespace Eras.Domain.Tests.Entities.Validators;

public class ProfessionalProfileValidatorTests
{
    private readonly ProfessionalProfileValidator _validator = new();

    [Fact]
    [Trait("Change", "603-persist-professional-profile")]
    [Trait("AC", "603-persist-professional-profile/AC-22")]
    public void Validate_OfficeHoursOver200Chars_FailsOnOfficeHours()
    {
        // Arrange (GIVEN) a profile with faculty "Science" and office hours of 201 characters
        var profile = new ProfessionalProfile
        {
            Faculty = "Science",
            OfficeHours = new string('x', 201),
        };

        // Act (WHEN) the profile is validated
        var result = _validator.TestValidate(profile);

        // Assert (THEN) the validation error names the office hours field
        result.ShouldHaveValidationErrorFor(Profile => Profile.OfficeHours);
    }
}
