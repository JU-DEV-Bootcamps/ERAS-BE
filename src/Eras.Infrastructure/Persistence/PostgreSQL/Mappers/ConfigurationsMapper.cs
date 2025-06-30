

using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{ 
    public static class ConfigurationsMapper
    {
        public static Domain.Entities.Configurations ToDomain(this ConfigurationsEntity Entity)
        {
            return new Domain.Entities.Configurations
            {
                Id = Entity.Id,
                UserId = Entity.UserId,
                ConfigurationName = Entity.ConfigurationName,
                BaseURL = Entity.BaseURL,
                EncryptedKey = Entity.EncryptedKey,
                ServiceProviderId = Entity.ServiceProviderId,
                Audit = Entity.Audit,
            };
        }

        public static ConfigurationsEntity ToPersistence(this Domain.Entities.Configurations Model)
        {
            return new ConfigurationsEntity
            {
                Id = Model.Id,
                UserId = Model.UserId,
                ConfigurationName = Model.ConfigurationName,
                BaseURL = Model.BaseURL,
                EncryptedKey = Model.EncryptedKey,
                ServiceProviderId = Model.ServiceProviderId,
                Audit = Model.Audit,
            };
        }
    }
}
