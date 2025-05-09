using Eras.Application.Mappers;
using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Tests.Mappers;
public class EvaluationMapperTest
{
    [Fact]
    public void ToDomain_Should_Convert_EvaluationDTO_To_Evaluation()
    {
        var dto = new EvaluationDTO()
        {
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow,
            Name = "Component",
            PollName = "PollName",
            Country = "CountryName"
        };
        var result = dto.ToDomain();
        Assert.NotNull(result);
        Assert.Equal(dto.StartDate, result.StartDate);
        Assert.Equal(dto.EndDate, result.EndDate);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.PollName, result.PollName);
        Assert.Equal(dto.Country, result.Country);
    }

    [Fact]
    public void ToDto_Should_Convert_Evaluation_To_EvaluationDto()
    {
        var model = new Evaluation()
        {
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow,
            Name = "Component",
            PollName = "PollName",
            Country = "CountryName"
        };
        var result = model.ToDto();
        Assert.NotNull(result);
        Assert.Equal(model.StartDate, result.StartDate);
        Assert.Equal(model.EndDate, result.EndDate);
        Assert.Equal(model.Name, result.Name);
        Assert.Equal(model.PollName, result.PollName);
        Assert.Equal(model.Country, result.Country);
    }
}
