using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities;
public class ServiceProvidersEntity : BaseEntity, IAuditableEntity
{
    public required string ServiceProviderName { get; set; }
    public required string ServiceProviderLogo { get; set; }
    public ICollection<ConfigurationsEntity> Configurations { get; set; }
    public AuditInfo Audit { get; set; }
}
