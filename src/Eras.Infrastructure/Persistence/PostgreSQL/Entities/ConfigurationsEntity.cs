using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities;
public class ConfigurationsEntity : BaseEntity, IAuditableEntity
{
    public required string UserId { get; set; }
    public required string ConfigurationName { get; set; }
    public required string BaseURL { get; set; }
    public required string EncryptedKey { get; set; }
    public int ServiceProviderId { get; set; }
    public ServiceProvidersEntity ServiceProvider { get; set; }
    public AuditInfo Audit { get; set; }
}
