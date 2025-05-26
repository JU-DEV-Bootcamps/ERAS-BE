using Eras.Application.DTOs;
using Eras.Domain.Common;

namespace Eras.Application.Dtos;

public class PollDTO
{
    public int Id { get; set; }
    public string IdCosmicLatte { get; set; } = string.Empty;
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime FinishedAt { get; set; }
    public int LastVersion { get; set; }
    public DateTime LastVersionDate { get; set; }
    public ICollection<ComponentDTO> Components { get; set; } = [];
    public AuditInfo? Audit { get; set; } = default!;
}

