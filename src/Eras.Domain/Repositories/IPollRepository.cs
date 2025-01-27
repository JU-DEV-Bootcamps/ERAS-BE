using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Domain.Repositories
{
    public interface IPollRepository<T>
    {
        Task<T> Add(T poll);
    }
}