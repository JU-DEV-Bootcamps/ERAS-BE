using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;
public interface IConfigurationsRepository : IBaseRepository<Configurations>
{
    Task<Configurations> GetByNameAsync(string ConfigurationName);
    Task<Configurations> GetByIdAsyncNoTracking(int ConfigurationId);
    Task<Configurations> UpdateDeleteStatus(int ConfigurationId);
    Task<List<Configurations>> GetUserConfigurationsAsync(string UserId);
}
