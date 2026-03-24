
namespace Eras.Domain.Abstractions;

public sealed class OperationResult<T>
{
    private readonly List<ValidationError> _errors = new();

    public bool IsSuccess => _errors.Count == 0;
    public T? Value { get; }
    public IReadOnlyCollection<ValidationError> Errors => _errors;

    private OperationResult(T value)
    {
        Value = value;
    }

    private OperationResult(IEnumerable<ValidationError> errors)
    {
        _errors.AddRange(errors);
    }

    public static OperationResult<T> Success(T value) => new(value);

    public static OperationResult<T> Failure(IEnumerable<ValidationError> errors) => new(errors);

    public static OperationResult<T> Failure(params ValidationError[] errors)
        => new(errors);
}
