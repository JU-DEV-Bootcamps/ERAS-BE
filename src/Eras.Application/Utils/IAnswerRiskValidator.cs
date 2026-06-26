using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Utils;

public interface IAnswerRiskValidator
{
    bool IsValidAnswer(string? AnswerText);
}
