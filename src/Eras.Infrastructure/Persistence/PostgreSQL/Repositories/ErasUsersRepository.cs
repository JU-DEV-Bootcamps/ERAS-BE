using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Mappers;
using Eras.Domain.Entities.UserManagement;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

public sealed class ErasUsersRepository(AppDbContext Context) : BaseRepository<ErasUser, ErasUser>
    (Context, X => X, X => X), IErasUsersRepository
{
    public async Task<ErasUserDTO?> GetErasUserByEmailAsync(string Email)
    {
        ErasUser? erasUser = await _context.ErasUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(User => User.Email == Email);

        return erasUser?.ToDTO();
    }

    public async Task<ErasUserDTO?> GetErasUserBySubAsync(string Sub)
    {
        ErasUser? erasUser = await _context.ErasUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(User => User.Sub == Sub);

        return erasUser?.ToDTO();
    }

    public async Task<IEnumerable<ErasUserDTO>> GetErasUsersByRoleAsync(string? Role)
    {
        IQueryable<ErasUser> query = _context.ErasUsers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(Role))
            query = query.Where(User => User.Role == Role);

        List<ErasUser> erasUsers = await query
            .OrderBy(User => User.FirstName)
            .ThenBy(User => User.LastName)
            .ToListAsync();

        return erasUsers.Select(User => User.ToDTO());
    }
}