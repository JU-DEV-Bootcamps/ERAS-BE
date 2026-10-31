using Eras.Application.DTOs.UserManuals;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers.UserManuals;

public sealed class UserManualMapper : IMapper<UserManual, UserManualDto>
{
    public UserManualDto Map(UserManual source) => new()
    {
        Id = source.Id,
        Title = source.Title,
        Language = source.Language,
        Version = source.Version,
        FileName = source.FileName,
        FileFormat = source.Format.ToString().ToUpperInvariant(),
    };
}
