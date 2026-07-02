using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly AppDbContext _context;
    public UserRoleRepository(AppDbContext context) => _context = context;

    public async Task<UserRoleEntity?> GetByEmailAsync(string email)
    {
        return await _context.UserRoles
            .AsNoTracking() 
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task AddAsync(UserRoleEntity userRole)
    {
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserRoleEntity userRole)
    {
        _context.UserRoles.Update(userRole);
        await _context.SaveChangesAsync();
    }
}