﻿using System.Diagnostics.CodeAnalysis;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.HeatMap;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class HeatMapRespository : IHeatMapRepository
    {
        protected readonly AppDbContext _context;

        public HeatMapRespository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetHeatMapByComponentsQueryResponse>> 
            GetHeatMapDataByComponentsAsync(string pollUUID)
        {
            var query = from v in _context.Variables
                        join c in _context.Components on v.ComponentId equals c.Id
                        join pv in _context.Set<PollVariableJoin>() on v.Id equals pv.VariableId
                        join a in _context.Answers on pv.Id equals a.PollVariableId
                        join pi in _context.PollInstances on a.PollInstanceId equals pi.Id
                        where pi.Uuid == pollUUID
                        select new GetHeatMapByComponentsQueryResponse
                        {
                            ComponentId = c.Id,
                            ComponentName = c.Name,
                            VariableId = v.Id,
                            VariableName = v.Name,
                            AnswerText = a.AnswerText,
                            AnswerRiskLevel = a.RiskLevel
                        };

            return await query.OrderBy(c => c.ComponentId)
                              .ThenBy(v => v.VariableId)
                              .ThenBy(a => a.AnswerText)
                              .ToListAsync();
        }

        public async Task<IEnumerable<GetHeatMapByComponentsQueryResponse>> 
            GetHeatMapDataByCohortAndDaysAsync(int? cohortId, int? days)
        {
            var pollInstances = await GetPollInstancesByCohortIdAndLastDays(cohortId, days);

            var pollInstanceIds = pollInstances.Select(pi => pi.Id).ToList();

            var query = from v in _context.Variables
                        join c in _context.Components on v.ComponentId equals c.Id
                        join pv in _context.Set<PollVariableJoin>() on v.Id equals pv.VariableId
                        join a in _context.Answers on pv.Id equals a.PollVariableId
                        join pi in _context.PollInstances.AsEnumerable() on a.PollInstanceId equals pi.Id
                        where pollInstanceIds.Contains(pi.Id)
                        select new GetHeatMapByComponentsQueryResponse
                        {
                            ComponentId = c.Id,
                            ComponentName = c.Name,
                            VariableId = v.Id,
                            VariableName = v.Name,
                            AnswerText = a.AnswerText,
                            AnswerRiskLevel = a.RiskLevel
                        };

            return await query.OrderBy(c => c.ComponentId)
                              .ThenBy(v => v.VariableId)
                              .ThenBy(a => a.AnswerText)
                              .ToListAsync();
        }

        internal async Task<IEnumerable<PollInstanceEntity>> 
            GetPollInstancesByCohortIdAndLastDays(int? cohortId, int? days)
        {
            IQueryable<PollInstanceEntity> query = _context.PollInstances
                .Include(pi => pi.Student)
                .Include(pi => pi.Answers);

            if (cohortId.HasValue && cohortId != 0)
            {
                query = query
                    .Join(_context.StudentCohorts,
                        pollInstance => pollInstance.StudentId,
                        studentCohort => studentCohort.StudentId,
                        (pollInstance, studentCohort) => new { pollInstance, studentCohort })
                    .Where(joined => joined.studentCohort.CohortId == cohortId.Value)
                    .Select(joined => joined.pollInstance);
            }

            if (days.HasValue && days != 0)
            {
                var dateLimit = DateTime.UtcNow.AddDays(-days.Value);
                query = query.Where(pi => pi.FinishedAt >= dateLimit);
            }

            return await query.Distinct().ToListAsync();
        }

        public async Task<IEnumerable<GetHeatMapAnswersPercentageByVariableQueryResponse>> GetHeatMapAnswersPercentageByVariableAsync(string pollUUID)
        {
            if (string.IsNullOrWhiteSpace(pollUUID))
            {
                throw new ArgumentException("El UUID de la encuesta no puede ser nulo o vacío.", nameof(pollUUID));
            }

            try
            {
                // Calcular el total de respuestas por poll_variable_id en la base de datos
                var totalAnswersByVariable = await _context.Answers
                    .Join(_context.PollVariables, a => a.PollVariableId, pv => pv.Id, (a, pv) => new { a, pv })
                    .Join(_context.Polls, apv => apv.pv.PollId, p => p.Id, (apv, p) => new { apv.a, apv.pv, p })
                    .Where(apvp => apvp.p.Uuid == pollUUID)
                    .GroupBy(apvp => apvp.a.PollVariableId)
                    .ToDictionaryAsync(
                        group => group.Key,
                        group => group.Count()
                    );

                // Obtener los datos y calcular el porcentaje en el cliente
                var query = await _context.Answers
                    .Join(_context.PollVariables, a => a.PollVariableId, pv => pv.Id, (a, pv) => new { a, pv })
                    .Join(_context.Variables, apv => apv.pv.VariableId, v => v.Id, (apv, v) => new { apv.a, apv.pv, v })
                    .Join(_context.Components, apv => apv.v.ComponentId, c => c.Id, (apv, c) => new { apv.a, apv.pv, apv.v, c })
                    .Join(_context.Polls, apv => apv.pv.PollId, p => p.Id, (apv, p) => new { apv.a, apv.v, apv.c, p })
                    .Where(apvp => apvp.p.Uuid == pollUUID)
                    .GroupBy(apvp => new
                    {
                        ComponentName = apvp.c.Name,   // Nombre único para Component.Name
                        PollVariableId = apvp.a.PollVariableId,
                        VariableName = apvp.v.Name,   // Nombre único para Variable.Name
                        apvp.a.AnswerText
                    })
                    .Select(group => new
                    {
                        group.Key.ComponentName,
                        group.Key.PollVariableId,
                        group.Key.VariableName,
                        group.Key.AnswerText,
                        AnswerCount = group.Count()
                    })
                    .OrderBy(result => result.PollVariableId)
                    .ThenByDescending(result => result.AnswerCount)
                    .ToListAsync();

                // Calcular el porcentaje en el cliente
                var results = query.Select(item => new GetHeatMapAnswersPercentageByVariableQueryResponse
                {
                    ComponentName = item.ComponentName,
                    PollVariableId = item.PollVariableId,
                    Name = item.VariableName,
                    AnswerText = item.AnswerText,
                    AnswerCount = item.AnswerCount,
                    Percentage = Math.Round((item.AnswerCount * 100.0) / totalAnswersByVariable[item.PollVariableId], 2)
                });

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al ejecutar la consulta para el UUID de encuesta: {pollUUID} el error es {ex}");
                throw new ApplicationException("Error al procesar los datos del mapa de calor.", ex);
            }
        }
    }
}

