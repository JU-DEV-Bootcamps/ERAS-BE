using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
public class ServiceProvidersRepository : BaseRepository<ServiceProviders, ServiceProvidersEntity>, IServiceProvidersRepository
{
    public ServiceProvidersRepository(AppDbContext Context)
            : base(Context, ServiceProvidersMapper.ToDomain, ServiceProvidersMapper.ToPersistence) { }

    public async Task<ServiceProviders?> GetByNameAsync(string ServiceProviderName)
    {
        var ServiceProvider = await _context.ServiceProviders.FirstOrDefaultAsync(ServiceProvider => ServiceProvider.ServiceProviderName == ServiceProviderName);

        return ServiceProvider?.ToDomain();
    }


}
