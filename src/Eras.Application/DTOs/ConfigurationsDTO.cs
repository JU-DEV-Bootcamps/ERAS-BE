using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Common;

namespace Eras.Application.DTOs;
public class ConfigurationsDTO
{
    [Required(ErrorMessage = "Configuration Id is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")]
    public int Id { get; set; }
    [Required(ErrorMessage = "UserId is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "UserId must be between 3 and 100 characters.")]
    public string UserId { get; set; }
    [Required(ErrorMessage = "Configuration name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Configuration name must be between 3 and 100 characters.")]
    public string ConfigurationName { get; set; }
    [Required(ErrorMessage = "Base URL is required.")]
    [Url(ErrorMessage = "Base URL must be a valid URL.")]
    [StringLength(200, ErrorMessage = "Base URL must be less than 200 characters.")]
    public string BaseURL { get; set; }
    [Required(ErrorMessage = "Encrypted key is required.")]
    [StringLength(200, MinimumLength = 10, ErrorMessage = "Encrypted key must be between 10 and 200 characters.")]
    public string EncryptedKey { get; set; }
    [Required(ErrorMessage = "Service provider Id is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "ServiceProviderId must be greater than 0.")]
    public int ServiceProviderId { get; set; }
    public bool IsDeleted { get; set; }
    public AuditInfo Audit { get; set; }
}
