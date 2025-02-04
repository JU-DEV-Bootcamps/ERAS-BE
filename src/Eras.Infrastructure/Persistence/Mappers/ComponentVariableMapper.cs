using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;

namespace Eras.Infrastructure.Persistence.Mappers
{
    public static class ComponentVariableMapper
    {
        public static ComponentVariableEntity ToComponentVariableEntity(this ComponentVariable componentVariable)
        {
            if (componentVariable == null) throw new ArgumentNullException(nameof(componentVariable));
            return new ComponentVariableEntity { 
                Id = componentVariable.Id, 
                Name = componentVariable.Name,
                PollId = componentVariable.PollId,
                Position = componentVariable.Position,
                ParentId = componentVariable.ParentId,
                CreatedDate = componentVariable.Audit.CreatedAt.ToUniversalTime(), 
                ModifiedDate = componentVariable.Audit.ModifiedAt.Value.ToUniversalTime()
            };
        }
        public static ComponentVariable ToComponentVariable(this ComponentVariableEntity componentVariableEntity)
        {
            if (componentVariableEntity == null) throw new ArgumentNullException(nameof(componentVariableEntity));
            return new ComponentVariable {
                Id = componentVariableEntity.Id,
                Name = componentVariableEntity.Name,
                PollId = componentVariableEntity.PollId,
                Position = componentVariableEntity.Position,
                ParentId = componentVariableEntity.ParentId,
                Audit = new AuditInfo {
                    CreatedAt = componentVariableEntity.CreatedDate.DateTime,
                    ModifiedAt = componentVariableEntity.ModifiedDate.DateTime
                }
            };
        }
    }
}