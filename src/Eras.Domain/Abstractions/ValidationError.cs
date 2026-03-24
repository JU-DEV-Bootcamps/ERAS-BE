namespace Eras.Domain.Abstractions;

public sealed record ValidationError
{
    public required string Code { get; init; }
    public required string Message { get; init; }
    public string? PropertyName { get; init; }
}
