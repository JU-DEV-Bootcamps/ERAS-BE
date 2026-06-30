using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Utils;

public class AnswerRiskValidator : IAnswerRiskValidator
{
    private static readonly HashSet<string> ExcludedAnswers = new(StringComparer.OrdinalIgnoreCase)
    {
        "-", "None", "Ninguno", "Ninguna"
    };

    public bool IsValidAnswer(string? AnswerText) =>
        !string.IsNullOrEmpty(AnswerText) && !ExcludedAnswers.Contains(AnswerText);
}
