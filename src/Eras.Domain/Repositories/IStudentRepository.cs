using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Domain.Entities;

namespace Eras.Domain.Repositories
{
    public interface IStudentRepository<T>
    {
        void Add(T student);
    }
}