using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;

namespace Eras.Application.DTOs;
public class ServiceProvidersDTO
{
    [Required(ErrorMessage = "Service provider name is required.")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Service provider name must be between 3 and 255 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-&.]+$", ErrorMessage = "Service provider name can only contain letters, numbers, spaces, dashes, dots, and the '&' symbol.")]
    public required string ServiceProviderName { get; set; }

    [Required(ErrorMessage = "Logo URL is required.")]
    [Url(ErrorMessage = "Logo URL must be a valid URL.")]
    [StringLength(10485760, ErrorMessage = "Logo URL must not exceed 10485760 characters.")]
    public required string ServiceProviderLogo { get; set; }

    public AuditInfo Audit { get; set; }
}
