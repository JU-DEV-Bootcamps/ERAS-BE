using Eras.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Contracts.Persistence
{
    public interface IStudentCohortRepository : IBaseRepository<Student>
    {
        Task<Student?> GetByCohortIdAndStudentIdAsync(int cohortId, int studentId);
        Task<IEnumerable<Student>?> GetAllStudentsByCohortIdAsync(int cohortId);
    }
}
