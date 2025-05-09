using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;

namespace Eras.Domain.Entities;
public class PollVersion: BaseEntity, IAuditableEntity
{
    public string Name = string.Empty;
    public DateTime Date;
    public int PollId { get; set; }
    public AuditInfo Audit { get; set; } = default!;
}
