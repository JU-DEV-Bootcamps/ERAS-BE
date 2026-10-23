using Eras.Application.DTOs;
using Eras.Domain.Entities.FeatureFlagManagement;

namespace Eras.Application.Mappers;

public static class FeatureFlagMapper
{
    public static FeatureFlag ToDomain(this FeatureFlagDTO DTO)
    {
        ArgumentNullException.ThrowIfNull(DTO);
        return new FeatureFlag
        {
            Id = DTO.Id ?? default,
            Name = DTO.Name,
            Description = DTO.Description,
            IsEnabled = DTO.IsEnabled,
            Audit = DTO.Audit
        };
    }

    public static FeatureFlagDTO ToDTO(this FeatureFlag Domain)
    {
        ArgumentNullException.ThrowIfNull(Domain);
        return new FeatureFlagDTO
        {
            Id = Domain.Id,
            Name = Domain.Name,
            Description = Domain.Description,
            IsEnabled = Domain.IsEnabled,
            Audit = Domain.Audit
        };
    }
}
