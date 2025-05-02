using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
public static class PollVersionMapper
{
    public static PollVersion ToDomain(this PollVersionEntity Entity)
    {
        return new PollVersion()
        {
            Id = Entity.Id,
            Name = Entity.Name,
            Date = Entity.Date,
            PollId = Entity.PollId,
            Audit = Entity.Audit,
        };
    }

    public static PollVersionEntity ToPersistence(this PollVersion Model)
    {
        return new PollVersionEntity()
        {
            Id = Model.Id,
            Name = Model.Name,
            Date = Model.Date,
            PollId = Model.PollId,
            Audit = Model.Audit,
        };
    }
}
