using System.ComponentModel.DataAnnotations;
using Eras.Domain.Common;

namespace Eras.Application.DTOs;

public class StudentDTO
{
    [Required(ErrorMessage = "Student Id is required.")]
    [Range(1, 2147483647, ErrorMessage = "Student Id must be greater than 0.")]
    public int Id { get; set; }

    [Required(ErrorMessage = "UUID is required.")]
    [RegularExpression(@"^[a-fA-F0-9\-]{36}$", ErrorMessage = "UUID format is invalid.")]
    public string Uuid { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Student name is required.")]
    [StringLength(254, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 254 characters.")]
    [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email is required.")]
    [StringLength(255, ErrorMessage = "Email must be less than 255 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email format is invalid.")]
    public string Email { get; set; } = string.Empty;
    
    public bool IsImported { get; set; }
    public StudentDetailDTO? StudentDetail { get; set; }
    public CohortDTO? Cohort { get; set; } = default!;
    public AuditInfo? Audit { get; set; } = default!;
}