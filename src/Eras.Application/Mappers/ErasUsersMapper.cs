using Eras.Application.DTOs.UsersManagement;
using Eras.Domain.Entities.UserManagement;

namespace Eras.Application.Mappers;
public static class ErasUsersMapper
{
    public static ErasUserDTO ToDTO(this ErasUser Entity)
    {
        ArgumentNullException.ThrowIfNull(Entity);
        return new ErasUserDTO
        {
            Id = Entity.Id,
            Sub = Entity.Sub,
            Email = Entity.Email,
            FirstName = Entity.FirstName,
            LastName = Entity.LastName,
            Role = Entity.Role,
            IsSynced = Entity.IsSynced,
            Audit = Entity.Audit
        };
    }

    public static ErasUser ToDomain(this ErasUserDTO DTO)
    {
        ArgumentNullException.ThrowIfNull(DTO);
        var entity = new ErasUser
        {
            Sub = DTO.Sub,
            Email = DTO.Email,
            FirstName = DTO.FirstName,
            LastName = DTO.LastName,
            Role = DTO.Role,
            IsSynced = DTO.IsSynced,
            Audit = DTO.Audit,
        };

        if (DTO.Id != null)
            entity.Id = (int)DTO.Id;
        
        return entity;
    }
}