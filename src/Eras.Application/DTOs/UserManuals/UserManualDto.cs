namespace Eras.Application.DTOs.UserManuals;

public sealed record UserManualDto
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Language { get; init; }
    public required string Version { get; init; }
    public required string FileName { get; init; }
    public required string FileFormat { get; init; }
}
