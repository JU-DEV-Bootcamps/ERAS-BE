using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities;
public class JURemissionEntity : BaseEntity, IAuditableEntity
{
    public required string SubmitterUuid { get; set; }
    public int JUServiceId { get; set; }
    public required JUServicesEntity JUService { get; set; }
    public required string AssignedProfessionalUuid { get; set; }
    public int StudentId { get; set; }
    public required StudentEntity Student { get; set; }
    public string Comment { get; set; } = string.Empty;
    public required DateTime Date { get; set; }
    public required string Status { get; set; } = RemissionsConstants.RemissionsStatus.Created.ToString();
    public required AuditInfo Audit { get; set; }
}
