using System.ComponentModel.DataAnnotations;
using Eras.Application.Attributes;
using Eras.Application.DTOs;
using Eras.Domain.Common;

namespace Eras.Application.Dtos;

public class PollDTO
{
    [Range(0, 2147483647, ErrorMessage = "Id must be zero or greater.")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Cosmic Latte Id is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Cosmic Latte Id must be between 3 and 100 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_\-]+$", ErrorMessage = "Cosmic Latte Id can only contain letters, numbers, dashes and underscores.")]
    public string IdCosmicLatte { get; set; } = string.Empty;

    [Required(ErrorMessage = "UUID is required.")]
    [RegularExpression(@"^[a-fA-F0-9\-]{36}$", ErrorMessage = "UUID format is invalid.")]
    public string Uuid { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
    [NoSqlInjection]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Finished date is required.")]
    [DataType(DataType.DateTime)]
    public DateTime FinishedAt { get; set; }

    [Range(1, 32767, ErrorMessage = "Last version must be at least 1.")]
    public int LastVersion { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime LastVersionDate { get; set; }

    [Required(ErrorMessage = "At least one component is required.")]
    [MinLength(1, ErrorMessage = "The poll must contain at least one component.")]
    public ICollection<ComponentDTO> Components { get; set; } = [];

    public AuditInfo? Audit { get; set; } = default!;

    [Required(ErrorMessage = "Parent Id is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Parent Id Id must be between 3 and 100 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_\-]+$", ErrorMessage = "Parent Id can only contain letters, numbers, dashes and underscores.")]
    public string ParentId { get; set; } = string.Empty;
}

