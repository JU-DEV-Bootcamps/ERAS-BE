namespace Eras.Api.Filters;

/// <summary>Body of an error response whose cause has a stable code the client can act on.</summary>
public sealed record ErrorCodeResponse(string Code, string Message);
