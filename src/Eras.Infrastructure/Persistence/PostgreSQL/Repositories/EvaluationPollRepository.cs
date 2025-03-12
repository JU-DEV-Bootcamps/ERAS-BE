using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    internal class EvaluationPollRepository: BaseRepository<Evaluation,EvaluationPollJoin>, IEvaluationPollRepository
    {
        public EvaluationPollRepository(AppDbContext context)
            : base(context, EvaluationPollMapper.ToDomain, EvaluationPollMapper.ToPersistence)
        {
        }
    }
}
