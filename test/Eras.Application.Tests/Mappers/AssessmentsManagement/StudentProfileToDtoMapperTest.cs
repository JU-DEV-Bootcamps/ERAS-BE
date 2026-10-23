using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Tests.Mappers.AssessmentsManagement;

public class StudentProfileToDtoMapperTest
{
    [Fact]
    public void Map_ShouldMapStudentProfileToDto()
    {
        Student source = new Student()
        {
            Id = 1,
            Name = "Abby",
            Email = "abby@gmail.com",
        };

        StudentProfileDto result = CreateSut().Map(source);

        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Name, result.Name);
        Assert.Equal(source.Email, result.Email);
    }

    private static StudentProfileToDtoMapper CreateSut()
    {
        return new StudentProfileToDtoMapper();
    }
}
