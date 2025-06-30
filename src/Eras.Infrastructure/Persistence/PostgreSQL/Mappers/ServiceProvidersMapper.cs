using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class ServiceProvidersMapper
    {
        public static Domain.Entities.ServiceProviders ToDomain(this Entities.ServiceProvidersEntity Entity)
        {
            return new Domain.Entities.ServiceProviders
            {
                Id = Entity.Id,
                ServiceProviderName = Entity.ServiceProviderName,
                ServiceProviderLogo = Entity.ServiceProviderLogo,
                Audit = Entity.Audit
            };
        }
        public static Entities.ServiceProvidersEntity ToPersistence(this Domain.Entities.ServiceProviders Model)
        {
            return new Entities.ServiceProvidersEntity
            {
                Id = Model.Id,
                ServiceProviderName = Model.ServiceProviderName,
                ServiceProviderLogo = Model.ServiceProviderLogo,
                Audit = Model.Audit
            };
        }
    }
}

