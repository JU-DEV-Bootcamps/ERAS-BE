using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;

namespace Eras.Application.DTOs;
public class ServiceProvidersDTO
{
    public required string ServiceProviderName { get; set; }
    public required string ServiceProviderLogo { get; set; }
    public AuditInfo Audit { get; set; }
}
