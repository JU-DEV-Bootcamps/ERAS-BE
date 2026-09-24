using Eras.Application.DTOs.UsersManagement;
using Eras.Domain.Entities.UserManagement;

namespace Eras.Application.Contracts.Persistence;
public interface IErasUsersRepository : IBaseRepository<ErasUser>
{
    Task<ErasUserDTO?> GetErasUserByEmailAsync(string Email);
    Task<ErasUserDTO?> GetErasUserBySubAsync(string Sub);
}