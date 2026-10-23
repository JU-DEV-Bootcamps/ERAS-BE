using Eras.Application.DTOs.AssessmentManagement;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities;

namespace Eras.Application.Tests.Mappers.AssessmentsManagement;

public class StudentProfileMapperTest
{
    [Fact]
    public void Map_ShouldMapStudentProfileToDto()
    {
        StudentProfileDto source = new StudentProfileDto()
        {
            Id = 1,
            Name = "Abby",
            Email = "abby@gmail.com",
            AvgRiskLevel = 1,
        };

        Student result = CreateSut().Map(source);

        Assert.Equal(source.Id, result.Id);
        Assert.Equal(source.Name, result.Name);
    }

    private static StudentProfileMapper CreateSut()
    {
        return new StudentProfileMapper();
    }
}
