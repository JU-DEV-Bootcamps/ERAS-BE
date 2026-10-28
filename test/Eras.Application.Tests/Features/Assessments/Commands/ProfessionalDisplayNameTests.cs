using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Domain.Common;

namespace Eras.Application.Tests.Features.Assessments.Commands;

public sealed class ProfessionalDisplayNameTests
{
    private static ErasUserDTO User(string FirstName, string LastName) => new()
    {
        Email = "ana@example.com",
        FirstName = FirstName,
        LastName = LastName,
        Role = "ERAS Professional",
        Audit = new AuditInfo()
    };

    [Fact]
    public void From_FirstAndLastName_ReturnsFullName()
    {
        Assert.Equal("Ana Pérez", ProfessionalDisplayName.From(User("Ana", "Pérez")));
    }

    [Fact]
    public void From_NamesAreBlank_ReturnsEmail()
    {
        Assert.Equal("ana@example.com", ProfessionalDisplayName.From(User(" ", "")));
    }
}
