﻿using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                CreatedDate = componentVariable.CreatedDate.ToUniversalTime(), 
                ModifiedDate = componentVariable.ModifiedDate.ToUniversalTime() 
            };
        }
    }
}