using Eras.Application.Contracts.Persistence;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
public class ConfigurationsRepository : BaseRepository<Domain.Entities.Configurations, ConfigurationsEntity>, IConfigurationsRepository
{
    public ConfigurationsRepository(AppDbContext Context)
            : base(Context, ConfigurationsMapper.ToDomain, ConfigurationsMapper.ToPersistence) { }

    public async Task<Domain.Entities.Configurations> GetByIdAsyncNoTracking(int ConfigurationId)
    {
        var configuration = await _context.Configurations.AsNoTracking().FirstOrDefaultAsync(Configuration => Configuration.Id == ConfigurationId);
        return configuration.ToDomain();
    }

    public async Task<Domain.Entities.Configurations?> GetByNameAsync(string ConfigurationName)
    {
        var configuration = await _context.Configurations.FirstOrDefaultAsync(Configuration => Configuration.ConfigurationName == ConfigurationName);

        return configuration?.ToDomain();
    }

    public async Task<List<Domain.Entities.Configurations>> GetUserConfigurationsAsync(string UserId)
    {
        var configuration = await _context.Configurations
                        .Where(c => c.UserId == UserId && c.IsDeleted == false)
                        .ToListAsync();

        var configurationDomain = configuration
         .Select(c => new Domain.Entities.Configurations
         {
             Id = c.Id,
             UserId = c.UserId,
             ServiceProviderId = c.ServiceProviderId,
             ConfigurationName = c.ConfigurationName,
             BaseURL = c.BaseURL,
             Audit = c.Audit,
             EncryptedKey = c.EncryptedKey,
             IsDeleted = c.IsDeleted
         })
         .ToList();

        return configurationDomain;
    }

    public Task<Domain.Entities.Configurations> UpdateDeleteStatus(int ConfigurationId)
    {
        var Configuration = _context.Configurations.FirstOrDefault(C => C.Id == ConfigurationId);
        if (Configuration != null)
        {
            Configuration.IsDeleted = true;
            _context.Configurations.Update(Configuration);
            _context.SaveChanges();
        }
        return Task.FromResult(Configuration.ToDomain());
    }
}
