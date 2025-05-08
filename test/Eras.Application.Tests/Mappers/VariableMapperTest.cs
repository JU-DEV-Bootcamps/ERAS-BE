using Eras.Application.Mappers;
using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Tests.Mappers;
public class VariableMapperTest
{
    [Fact]
    public void ToDomain_Should_Convert_VariableDTO_To_Variable()
    {
        var dto = new VariableDTO()
        { 
           Name = "Name",
           Type = "Type",
        };
        var result = dto.ToDomain();
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Type, result.Type);
    }

    [Fact]
    public void ToDto_Should_Convert_Variable_To_VariableDto()
    {
        var model = new Variable()
        {
            Name = "Name",
            Type = "Type",
        };
        var result = model.ToDto();
        Assert.NotNull(result);
        Assert.Equal(model.Name, result.Name);
        Assert.Equal(model.Type, result.Type);
    }
}
