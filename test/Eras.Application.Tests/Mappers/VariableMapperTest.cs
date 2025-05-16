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
        };
        var result = dto.ToDomain();
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
    }

    [Fact]
    public void ToDto_Should_Convert_Variable_To_VariableDto()
    {
        var model = new Variable()
        {
            Name = "Name",
        };
        var result = model.ToDto();
        Assert.NotNull(result);
        Assert.Equal(model.Name, result.Name);
    }
}
