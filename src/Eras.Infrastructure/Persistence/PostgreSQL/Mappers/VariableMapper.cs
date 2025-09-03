using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

public static class VariableMapper
{
    public static Variable ToDomain(this VariableEntity Entity) => new()
    {
        Id = Entity.Id,
        Name = Entity.Name,
        Position = Entity.Position,
        Audit = Entity.Audit,
    };
    public static VariableEntity ToPersistence(this Variable Model) => new()
    {
        Id = Model.Id,
        Name = Model.Name,
        Position = Model.Position,
        Audit = Model.Audit,
        ComponentId = Model.IdComponent
    };
}
