using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;
public interface IPollVersionRepository: IBaseRepository<PollVersion>
{
    Task<PollVersion?> GetByPollAndVersionAsync(string VersionName, int PollId);
    Task<List<PollVersion>> GetAllByPollAsync(int PollId);
}
