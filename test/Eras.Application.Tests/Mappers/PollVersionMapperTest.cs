using Eras.Application.Mappers;
using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Tests.Mappers;
public class PollVersionMapperTest
{
    [Fact]
    public void ToDomain_Should_Convert_PollVersionDTO_To_PollVersion()
    {
        var dto = new PollVersionDTO()
        {
            Name = "name",
            Date = DateTime.Now,
        };
        var result = dto.ToDomain();
        Assert.NotNull(result);
        Assert.Equal(dto.Date, result.Date);
        Assert.Equal(dto.Name, result.Name);
    }

    [Fact]
    public void ToDto_Should_Convert_PollVersion_To_PollVersionDto()
    {
        var model = new PollVersion()
        {
            Date = DateTime.Now,
            Name = "name"
        };
        var result = model.ToDto();
        Assert.NotNull(result);
        Assert.Equal(model.Date, result.Date);
        Assert.Equal(model.Name, result.Name);
    }
}
