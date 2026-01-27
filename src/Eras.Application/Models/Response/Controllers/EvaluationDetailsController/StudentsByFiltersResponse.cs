using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Models.Response.Controllers.EvaluationDetailsController;

public class StudentsByFiltersResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int AnswerId { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public decimal RiskLevel { get; set; }
    
}
