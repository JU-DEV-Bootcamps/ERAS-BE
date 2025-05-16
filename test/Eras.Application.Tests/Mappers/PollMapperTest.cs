using Eras.Application.Dtos;
using Eras.Domain.Entities;
using Eras.Application.Mappers;
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
            Name= "name"
        };
        var result = model.ToDto();
        Assert.NotNull(result);
        Assert.Equal(model.Uuid, result.Uuid);
        Assert.Equal(model.Name, result.Name);
    }
}
