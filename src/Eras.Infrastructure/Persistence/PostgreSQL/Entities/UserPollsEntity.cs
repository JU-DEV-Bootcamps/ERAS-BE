using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities;
public class UserPollsEntity : BaseEntity, IAuditableEntity
{
    public required string UserId { get; set; }
    public int PollId { get; set; }
    public PollEntity Poll { get; set; }
    public int ConfigurationId { get; set; }
    public ConfigurationsEntity Configuration { get; set; }
    public AuditInfo Audit { get; set; }
}
