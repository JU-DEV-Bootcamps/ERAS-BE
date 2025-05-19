using System;
using Eras.Application.Mappers;
using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Tests.Mappers;
public class CohortMapperTest
{
    [Fact]
    public void ToDomain_Should_Convert_CohortDTO_To_Cohort()
    {
        var dto = new CohortDTO
        {
            Name = "Cohort",
            CourseCode = "123",
        };
        var result = dto.ToDomain();
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.CourseCode, result.CourseCode);
    }

    [Fact]
    public void ToDto_Should_Convert_Cohort_To_CohortDto()
    {
        var model = new Cohort
        {
            Name = "Cohort",
            CourseCode = "123",
        };
        var result = model.ToDto();
        Assert.NotNull(result);
        Assert.Equal(model.Name, result.Name);
        Assert.Equal(model.CourseCode, result.CourseCode);
    }
}
