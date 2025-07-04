using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
public class ServiceProvidersRepository : BaseRepository<Domain.Entities.ServiceProviders, ServiceProvidersEntity>, IServiceProvidersRepository
{
    public ServiceProvidersRepository(AppDbContext Context)
            : base(Context, ServiceProvidersMapper.ToDomain, ServiceProvidersMapper.ToPersistence) { }

    public async Task<Domain.Entities.ServiceProviders?> GetByNameAsync(string ServiceProviderName)
    {
        var ServiceProvider = await _context.ServiceProviders.FirstOrDefaultAsync(ServiceProvider => ServiceProvider.ServiceProviderName == ServiceProviderName);

        return ServiceProvider?.ToDomain();
    }


}
