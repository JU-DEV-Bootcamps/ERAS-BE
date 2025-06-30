using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Models.Response.HeatMap;
using Eras.Domain.Entities;
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
                        .Where(c => c.UserId == UserId)
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
             EncryptedKey = c.EncryptedKey
         })
         .ToList();

        return configurationDomain;
    }

}
