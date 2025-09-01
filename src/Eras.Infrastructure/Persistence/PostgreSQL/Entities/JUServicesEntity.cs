using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities;
public class JUServicesEntity : BaseEntity, IAuditableEntity
{
    public required string ServiceName { get; set; }
    public required AuditInfo Audit { get; set; }
}
