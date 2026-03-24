
namespace Eras.Domain.Abstractions;

public sealed class OperationResult
{
    private readonly List<ValidationError> _errors = [];

    public bool IsSuccess => _errors.Count == 0;
    public IReadOnlyCollection<ValidationError> Errors => _errors;

    public static OperationResult Success() => new();

    public static OperationResult Failure(IEnumerable<ValidationError> errors)
    {
        var result = new OperationResult();
        result._errors.AddRange(errors);
        return result;
    }

    public static OperationResult Failure(params ValidationError[] errors)
        => Failure((IEnumerable<ValidationError>)errors);

    public static OperationResult Merge(params OperationResult[] results)
    {
        List<ValidationError> errors = results
            .Where(x => !x.IsSuccess)
            .SelectMany(x => x.Errors)
            .ToList();

        return errors.Count == 0
            ? Success()
            : Failure(errors);
    }
}