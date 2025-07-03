using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;

namespace Eras.Application.DTOs;
public class ConfigurationsDTO
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string ConfigurationName { get; set; }
    public string BaseURL { get; set; }
    public string EncryptedKey { get; set; }
    public int ServiceProviderId { get; set; }
    public bool IsDeleted { get; set; }
    public AuditInfo Audit { get; set; }
}
