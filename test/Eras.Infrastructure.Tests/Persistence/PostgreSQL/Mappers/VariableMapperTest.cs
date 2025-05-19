using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Domain.Entities;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Mappers;
public class VariableMapperTest
{
    [Fact]
    public void ToDomain_Should_Convert_VariableEntity_To_Variable()
    {
        var entity = new VariableEntity
        {
            Id = 1,
            Name = "Variable"
        };

        var result = entity.ToDomain();
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.Name, result.Name);
    }

    [Fact]
    public void ToPersistenceVariable_Should_Convert_Variable_To_VariableEntity()
    {
        var model = new Variable
        {
            Id = 2,
            Name = "Variable"
        };
        var result = model.ToPersistence();
        Assert.NotNull(result);
        Assert.Equal(model.Id, result.Id);
        Assert.Equal(model.Name, result.Name);
    }
}
