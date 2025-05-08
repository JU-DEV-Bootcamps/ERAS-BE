using Eras.Application.Mappers;
using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Tests.Mappers;
public class ComponentMapperTest
{
    [Fact]
    public void ToDomain_Should_Convert_ComponentDTO_To_Component()
    {
        var dto = new ComponentDTO
        {
            Name = "Component",
        };
        var result = dto.ToDomain();
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
    }

    [Fact]
    public void ToDto_Should_Convert_Component_To_ComponentDto()
    {
        var model = new Component
        {
            Name = "Component",
        };
        var result = model.ToDto();
        Assert.NotNull(result);
        Assert.Equal(model.Name, result.Name);
    }
}
