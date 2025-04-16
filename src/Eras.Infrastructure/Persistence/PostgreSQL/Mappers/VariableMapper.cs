using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

public static class VariableMapper
{
    public static Variable ToDomain(this VariableEntity Entity) => new()
    {
        Id = Entity.Id,
        Name = Entity.Name,
        Audit = Entity.Audit,
        Component = Entity.Component?.ToDomain() ?? new Component(),
    };
    public static VariableEntity ToPersistence(this Variable Model) => new()
    {
        Id = Model.Id,
        Name = Model.Name,
        Audit = Model.Audit,
        ComponentId = Model.IdComponent
    };
}
