using Eras.Domain.Common;

namespace Eras.Application.DTOs;

public sealed class FeatureFlagDTO
{
    public int? Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public bool IsEnabled { get; set; } = false;
    public required AuditInfo Audit { get; set; }
}