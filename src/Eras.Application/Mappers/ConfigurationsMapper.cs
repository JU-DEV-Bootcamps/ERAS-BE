using Eras.Application.DTOs;
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers;
public static class ConfigurationsMapper
{
    public static Configurations ToDomain(this ConfigurationsDTO Dto)
    {
        ArgumentNullException.ThrowIfNull(Dto);
        return new Configurations
        {
            UserId = Dto.UserId,
            ConfigurationName = Dto.ConfigurationName,
            BaseURL = Dto.BaseURL,
            EncryptedKey = Dto.EncryptedKey,
            ServiceProviderId = Dto.ServiceProviderId,
            IsDeleted = Dto.IsDeleted,
            Audit = Dto.Audit ?? new AuditInfo()
            {
                CreatedBy = "Configurations Mapper",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
            },
        };
    }
    public static ConfigurationsDTO ToDto(this Configurations Domain)
    {
        ArgumentNullException.ThrowIfNull(Domain);
        return new ConfigurationsDTO
        {
            UserId = Domain.UserId,
            ConfigurationName = Domain.ConfigurationName,
            BaseURL = Domain.BaseURL,
            EncryptedKey = Domain.EncryptedKey,
            ServiceProviderId = Domain.ServiceProviderId,
            IsDeleted = Domain.IsDeleted,
            Audit = Domain.Audit,
        };
    }
}
