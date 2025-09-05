using Eras.Domain.Common;

namespace Eras.Domain.Entities;
public class ServiceProviders : BaseEntity, IAuditableEntity
{
    public required string ServiceProviderName { get; set; }
    public required string ServiceProviderLogo { get; set; }
    public ICollection<Configurations> Configurations { get; set; }
    public AuditInfo Audit { get; set; }
}
