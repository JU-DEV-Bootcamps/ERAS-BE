using Eras.Application.Dtos;
using Eras.Domain.Entities;
using Eras.Application.Mappers;
using Eras.Application.DTOs;
namespace Eras.Application.Tests.Mappers;
public class PollMapperTest
{
    [Fact]
    public void ToDomain_Should_Convert_PollDTO_To_Poll()
    {
        var dto = new PollDTO()
        {
            Uuid = "1234",
            Name = "name"
        };
        var result = dto.ToDomain();
        Assert.NotNull(result);
        Assert.Equal(dto.Uuid, result.Uuid);
        Assert.Equal(dto.Name, result.Name);
    }

    [Fact]
    public void ToDto_Should_Convert_PollInstance_To_PollInstanceDto()
    {
        var model = new Poll()
        {
            Uuid = "1234",
            Name= "name",
            Components = null!
        };
        var result = model.ToDto();
        Assert.NotNull(result);
        Assert.Equal(model.Uuid, result.Uuid);
        Assert.Equal(model.Name, result.Name);
        Assert.Empty(result.Components);
    }

    [Fact]
    public void ToDto_Should_ConvertPollInstance_WithComponentsProvided()
    {
        var model = new Poll()
        {
            Uuid = "1234",
            Name = "name",
            Components = new List<Component>
            {
                new Component 
                {
                    Id = 1,
                }
            }
        };
        var result = model.ToDto();
        Assert.NotNull(result);
        Assert.Equal(model.Uuid, result.Uuid);
        Assert.Equal(model.Name, result.Name);
        Assert.Equal(1, result.Components.Count);
    }
}
