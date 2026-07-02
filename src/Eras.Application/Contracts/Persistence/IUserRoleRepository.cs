using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface IUserRoleRepository
{
    Task<UserRoleEntity?> GetByEmailAsync(string email);
    Task AddAsync(UserRoleEntity userRole);
    Task UpdateAsync(UserRoleEntity userRole);
}