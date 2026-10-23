using Eras.Domain.Common;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities;
public class UserPollsEntity : BaseEntity, IAuditableEntity
{
    public required string UserId { get; set; }
    public int PollId { get; set; }
    public PollEntity Poll { get; set; } = null!;
    public int ConfigurationId { get; set; }
    public ConfigurationsEntity Configuration { get; set; } = null!;
    public AuditInfo Audit { get; set; } = null!;
}
