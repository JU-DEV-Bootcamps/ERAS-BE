using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class StudentCohortRepository : BaseRepository<Student, StudentCohortJoin>, IStudentCohortRepository
    {
        public StudentCohortRepository(AppDbContext context)
            : base(context, StudentCohortMapper.ToDomain, StudentCohortMapper.ToPersistenceVariable)
        {

        }
    }
}