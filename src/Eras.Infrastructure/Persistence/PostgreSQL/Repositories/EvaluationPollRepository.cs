﻿using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    internal class EvaluationPollRepository(AppDbContext context) : BaseRepository<Evaluation, EvaluationPollJoin>(context, EvaluationPollMapper.ToDomain, EvaluationPollMapper.ToPersistence), IEvaluationPollRepository
    {
    }
}
