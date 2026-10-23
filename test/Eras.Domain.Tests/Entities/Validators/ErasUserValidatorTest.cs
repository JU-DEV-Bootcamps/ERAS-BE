using Eras.Domain.Common;
using Eras.Domain.Entities.UserManagement;
using Eras.Domain.Tests.TestUtils;

using FluentValidation.TestHelper;

namespace Eras.Domain.Tests.Entities.Validators;
public class ErasUserValidatorTests
{
    private readonly ErasUserValidator _validator = new();

    private static ErasUser CreateErasUser(
        string Email = "mario@test.com",
        string FirstName = "Mario",
        string LastName = "Martinez",
        string Role = "ERAS Administrator",
        string? Sub = "1e9f0af2-e3e7-4069-8e14-764d7e7b7526",
        AuditInfo? Audit = null
    ) => new ()
    {
      Email = Email,
      FirstName = FirstName,
      LastName = LastName,
      Role = Role,
      Sub = Sub,
      Audit = Audit ?? new AuditInfo()  
    };

    [Fact]
    public void Should_NotHaveValidationErrors_When_ErasUserIsValid()
    {
        ErasUser user = CreateErasUser();

        TestValidationResult<ErasUser> result = _validator.TestValidate(user);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void Should_HaveValidationErrors_When_Email_IsNotProvided(string Email)
    {
        ErasUser user = CreateErasUser(Email: Email);

        TestValidationResult<ErasUser> result = _validator.TestValidate(user);

        result.ShouldHaveValidationErrorFor(U => U.Email)
            .WithErrorMessage("Email is required.");
    }

    [Theory]
    [ClassData(typeof(EmailFormatTestData))]
    public void Should_HaveValidationErrors_When_Email_HasWrongFormat(string Email)
    {
        ErasUser user = CreateErasUser(Email: Email);

        TestValidationResult<ErasUser> result = _validator.TestValidate(user);

        result.ShouldHaveValidationErrorFor(U => U.Email)
            .WithErrorMessage("Email is not valid.");
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void Should_HaveValidationErrors_When_FirstName_IsNotProvided(string FirstName)
    {
        ErasUser user = CreateErasUser(FirstName: FirstName);

        TestValidationResult<ErasUser> result = _validator.TestValidate(user);

        result.ShouldHaveValidationErrorFor(U => U.FirstName)
            .WithErrorMessage("First name is required.");
    }

    [Fact]
    public void Should_HaveValidationErrors_When_FirstName_IsTooLong()
    {
        var name = new string('a', 101);
        ErasUser user = CreateErasUser(FirstName: name);

        TestValidationResult<ErasUser> result = _validator.TestValidate(user);

        result.ShouldHaveValidationErrorFor(U => U.FirstName)
            .WithErrorMessage("First name must be under 100 characters.");
    }

    [Theory]
    [ClassData(typeof(RequiredStringTestData))]
    public void Should_HaveValidationErrors_When_LastName_IsNotProvided(string LastName)
    {
        ErasUser user = CreateErasUser(LastName: LastName);

        TestValidationResult<ErasUser> result = _validator.TestValidate(user);

        result.ShouldHaveValidationErrorFor(U => U.LastName)
            .WithErrorMessage("Last name is required.");
    }

    [Fact]
    public void Should_HaveValidationErrors_When_LastName_IsTooLong()
    {
        var name = new string('a', 101);
        ErasUser user = CreateErasUser(LastName: name);

        TestValidationResult<ErasUser> result = _validator.TestValidate(user);

        result.ShouldHaveValidationErrorFor(U => U.LastName)
            .WithErrorMessage("Last name must be under 100 characters.");
    }

    [Fact]
    public void Should_HaveValidationErrors_When_RoleIsNotErasRole()
    {
        ErasUser user = CreateErasUser(Role: "Super User");

        TestValidationResult<ErasUser> result = _validator.TestValidate(user);

        result.ShouldHaveValidationErrorFor(U => U.Role)
            .WithErrorMessage("Super User is not a valid Eras role.");
    }

    [Theory]
    [ClassData(typeof(ValidErasRolesTestData))]
    public void Should_NotHaveValidationErrors_When_RoleIsValidErasRole(string role)
    {
        ErasUser user = CreateErasUser(Role: role);

        TestValidationResult<ErasUser> result = _validator.TestValidate(user);

        result.ShouldNotHaveValidationErrorFor(U => U.Role);
    }
}
