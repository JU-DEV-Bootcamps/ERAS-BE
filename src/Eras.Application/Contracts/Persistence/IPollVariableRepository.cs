using Eras.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Contracts.Persistence
{
    public interface IPollVariableRepository: IBaseRepository<Variable>
    {
        Task<Variable?> GetByPollIdAndVariableIdAsync(int pollId, int variableId);   
    }
}
