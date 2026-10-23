using Eras.Domain.Entities;

namespace Eras.Application.Validation;
public static class EvaluationDateRangeValidator
{
    public static void EnsureWithinRange(Evaluation evaluation, DateTime requestedStart, DateTime requestedEnd)
    {
        bool outOfRange = evaluation.EndDate < requestedStart || evaluation.StartDate > requestedEnd;
        if (outOfRange)
        {
            throw new ArgumentException(
                $"There was an error during the import: The selected date range " +
                $"({requestedStart:yyyy-MM-dd} - {requestedEnd:yyyy-MM-dd}) is outside the evaluation's " +
                $"date range ({evaluation.StartDate:yyyy-MM-dd} - {evaluation.EndDate:yyyy-MM-dd}).");
        }
    }
}