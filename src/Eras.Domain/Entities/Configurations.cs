using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;

namespace Eras.Domain.Entities;
public class Configurations : BaseEntity, IAuditableEntity
{
    public string UserId { get; set; }
    public string ConfigurationName { get; set; }
    public string BaseURL { get; set; }
    public string EncryptedKey { get; set; }
    public int ServiceProviderId { get; set; }
    public bool IsDeleted { get; set; }
    public ServiceProviders ServiceProvider { get; set; }
    public AuditInfo Audit { get; set; }
}