using System.ComponentModel.DataAnnotations;

using Eras.Domain.Common;

namespace Eras.Application.DTOs;
public class ConfigurationsDTO
{
    [Required(ErrorMessage = "Configuration Id is required.")]
    [Range(0, 2147483647, ErrorMessage = "Id must be greater or equal to 0.")]
    public int Id { get; set; }

    [Required(ErrorMessage = "UserId is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "UserId must be between 3 and 100 characters.")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "Configuration name is required.")]
    [StringLength(10485760, MinimumLength = 3, ErrorMessage = "Configuration Name must be greater than 0.")]
    public string ConfigurationName { get; set; }

    [Required(ErrorMessage = "Base URL is required.")]
    [Url(ErrorMessage = "Base URL must be a valid URL.")]
    [StringLength(500, ErrorMessage = "Base URL must be less than 501 characters.")]
    public string BaseURL { get; set; }

    [Required(ErrorMessage = "Encrypted key is required.")]
    [StringLength(10485760, MinimumLength = 10, ErrorMessage = "Encrypted Key must be greater than 10.")]
    public string EncryptedKey { get; set; }

    [Required(ErrorMessage = "Service provider Id is required.")]
    [Range(0, 2147483647, ErrorMessage = "Service Provider Id must be greater or equal to 0.")]
    public int ServiceProviderId { get; set; }

    public bool IsDeleted { get; set; }
    public AuditInfo Audit { get; set; }
}
