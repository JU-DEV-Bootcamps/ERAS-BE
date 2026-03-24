using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.DTOs;

public class DashboardKpiDto
{
    public KpiMetricDto TotalStudents { get; set; } = new();
    public KpiMetricDto TotalPollsAnswered { get; set; } = new();
    public KpiMetricDto TotalEvaluations { get; set; } = new();
}

public class KpiMetricDto
{
    public int Value { get; set; }
    public double PercentageChange { get; set; }
}
