using Eras.Domain.Common;

namespace Eras.Domain.Entities;
public class Configurations : BaseEntity, IAuditableEntity
{
    public required string UserId { get; set; }
    public required string ConfigurationName { get; set; }
    public required string BaseURL { get; set; }
    public required string EncryptedKey { get; set; }
    public int ServiceProviderId { get; set; }
    public bool IsDeleted { get; set; }
    public ServiceProviders ServiceProvider { get; set; } = null!;
    public AuditInfo Audit { get; set; } = null!;
}