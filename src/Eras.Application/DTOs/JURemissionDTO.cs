using System.ComponentModel.DataAnnotations;

using Eras.Domain.Common;

using static Eras.Domain.Entities.JURemissionsConstants;

namespace Eras.Application.DTOs;
public class JURemissionDTO 
{
    [Required(ErrorMessage = "JURemission Id is required.")]
    [Range(0, 2147483647, ErrorMessage = "Id must be greater than or equals 0.")]
    public int Id { get; set; }

    [Required(ErrorMessage = "JURemission SubmitterID is required.")]
    [StringLength(36, MinimumLength = 36, ErrorMessage = "JURemission Submitter UUID must be exactly 36 characters.")]
    [RegularExpression(@"^[a-fA-F0-9\-]{36}$", ErrorMessage = "JURemission UUID must follow a valid GUID format.")]
    public string SubmitterUuid { get; set; } = string.Empty;
    public int JUServiceId { get; set; } = default!;
    public int AssignedProfessionalId { get; set; } = default!;

    [StringLength(255, MinimumLength = 3, ErrorMessage = "JURemission Comment must be at least 3 characters long and at most 255")]
    public string Comment { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public RemissionsStatus Status { get; set; } = RemissionsStatus.Created;
    public ICollection<StudentDTO> Students { get; set; } = [];
    public AuditInfo? Audit { get; set; } = default!;
}
