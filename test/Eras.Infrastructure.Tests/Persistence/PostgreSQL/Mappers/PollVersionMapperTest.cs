using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Mappers;
public class PollVersionMapperTest
{
    [Fact]
    public void ToDomain_Should_Convert_PollVersionEntity_To_PollVersion()
    {
        // Arrange
        var entity = new PollVersionEntity
        {
            Id = 1,
            Name = "Test Version",
            Date = DateTime.Now,
        };
        var result = entity.ToDomain();
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.Name, result.Name);
        Assert.Equal(entity.Date, result.Date);
    }

    [Fact]
    public void ToPersistence_Should_Convert_PollVersion_To_PollVersionEntity()
    {
        var model = new PollVersion
        {
            Id = 1,
            Name = "Test Version",
            Date = DateTime.Now,
        };
        var result = model.ToPersistence();
        Assert.NotNull(result);
        Assert.Equal(model.Id, result.Id);
        Assert.Equal(model.Name, result.Name);
        Assert.Equal(model.Date, result.Date);
    }
}
