using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;

namespace Eras.Domain.Entities;
public class ServiceProviders : BaseEntity, IAuditableEntity
{
    public required string ServiceProviderName { get; set; }
    public required string ServiceProviderLogo { get; set; }
    public ICollection<Configurations> Configurations { get; set; }
    public AuditInfo Audit { get; set; }
}
