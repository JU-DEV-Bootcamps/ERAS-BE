﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Domain.Repositories
{
    public interface IComponentVariableRepository<T>
    {
        Task<T> Add (T componentVariable);
    }
}
