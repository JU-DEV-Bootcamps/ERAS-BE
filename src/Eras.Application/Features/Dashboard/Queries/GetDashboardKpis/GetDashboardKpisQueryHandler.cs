using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Dashboard.Queries.GetDashboardKpis;

class GetDashboardKpisQueryHandler(
    IStudentRepository StudentRepository,
    IPollInstanceRepository PollInstanceRepository,
    IEvaluationRepository EvaluationRepository,
    ILogger<GetDashboardKpisQueryHandler> Logger
) : IRequestHandler<GetDashboardKpisQuery, GetQueryResponse<DashboardKpiDto>>
{
    private readonly IStudentRepository _studentRepository = StudentRepository;
    private readonly IPollInstanceRepository _pollInstanceRepository = PollInstanceRepository;
    private readonly IEvaluationRepository _evaluationRepository = EvaluationRepository;
    private readonly ILogger<GetDashboardKpisQueryHandler> _logger = Logger;

    async Task<GetQueryResponse<DashboardKpiDto>> IRequestHandler<GetDashboardKpisQuery, GetQueryResponse<DashboardKpiDto>>.Handle(GetDashboardKpisQuery Request, CancellationToken CancellationToken)
    {
        try
        {
            var currentRange = CohortsHelper.GetCurrentCohortRange();
            var prevRange = CohortsHelper.GetPreviousCohortRange();

            var totalStud = await _studentRepository.CountAsync();
            var currStud = await _studentRepository.CountByDateRangeAsync(currentRange.Start, currentRange.End);
            var prevStud = await _studentRepository.CountByDateRangeAsync(prevRange.Start, prevRange.End);

            var totalPolls = await _pollInstanceRepository.CountAsync();
            var currPolls = await _pollInstanceRepository.CountByDateRangeAsync(currentRange.Start, currentRange.End);
            var prevPolls = await _pollInstanceRepository.CountByDateRangeAsync(prevRange.Start, prevRange.End);

            var totalEval = await _evaluationRepository.CountAsync();
            var currEval = await _evaluationRepository.CountByDateRangeAsync(currentRange.Start, currentRange.End);
            var prevEval = await _evaluationRepository.CountByDateRangeAsync(prevRange.Start, prevRange.End);

            var result = new DashboardKpiDto
            {
                TotalStudents = new KpiMetricDto { Value = totalStud, PercentageChange = CalculateChange(currStud, prevStud) },
                TotalPollsAnswered = new KpiMetricDto { Value = totalPolls, PercentageChange = CalculateChange(currPolls, prevPolls) },
                TotalEvaluations = new KpiMetricDto { Value = totalEval, PercentageChange = CalculateChange(currEval, prevEval) }
            };

            return new GetQueryResponse<DashboardKpiDto>(result, "KPIs calculated successfully", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dashboard KPI Error");
            return new GetQueryResponse<DashboardKpiDto>(null, "Error", false);
        }
    }

    private static double CalculateChange(int current, int previous)
    {
        if (previous == 0) return current > 0 ? 100.0 : 0.0;
        double diff = current - previous;
        return Math.Round((diff / previous) * 100, 2);
    }
}