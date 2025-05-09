using Eras.Application.DTOs;
using Eras.Application.Mappers;
using Eras.Domain.Entities;
namespace Eras.Application.Tests.Mappers;
public class PollInstanceMapperTest
{
    [Fact]
    public void ToDomain_Should_Convert_PollInstanceDTO_To_PollInstace()
    {
        var dto = new PollInstanceDTO()
        {
            Uuid = "1234",
            FinishedAt = DateTime.Now,
        };
        var result = dto.ToDomain();
        Assert.NotNull(result);
        Assert.Equal(dto.Uuid, result.Uuid);
        Assert.Equal(dto.FinishedAt, result.FinishedAt);
    }

    [Fact]
    public void ToDto_Should_Convert_PollInstance_To_PollInstanceDto()
    {
        var model = new PollInstance()
        {
            Uuid = "1234",
            FinishedAt = DateTime.Now,
        };
        var result = model.ToDTO();
        Assert.NotNull(result);
        Assert.Equal(model.Uuid, result.Uuid);
        Assert.Equal(model.FinishedAt, result.FinishedAt);
    }
}
