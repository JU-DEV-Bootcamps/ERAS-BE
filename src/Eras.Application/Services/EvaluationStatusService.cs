using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

namespace Eras.Application.Services;

public static class EvaluationStatusService
{
    public static EvaluationConstants.EvaluationStatus ComputeStatus(Evaluation evaluation)
    {
        if (evaluation.Polls == null || evaluation.Polls.Count == 0)
        {
            return EvaluationConstants.EvaluationStatus.Pending;
        }

        bool hasAnswers = evaluation.PollInstances != null && 
            evaluation.PollInstances.Any(pi => pi.Answers.Count > 0 || pi.SourcePollInstanceId != null);

        var now = DateTime.UtcNow;

        if (!hasAnswers)
        {
            return now > evaluation.EndDate
                ? EvaluationConstants.EvaluationStatus.Uncompleted
                : EvaluationConstants.EvaluationStatus.Ready;
        }
        return now > evaluation.EndDate
            ? EvaluationConstants.EvaluationStatus.Completed
            : EvaluationConstants.EvaluationStatus.InProgress;
    }
}
