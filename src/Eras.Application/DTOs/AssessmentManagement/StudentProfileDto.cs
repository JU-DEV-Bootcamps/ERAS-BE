namespace Eras.Application.DTOs.AssessmentManagement;

public sealed record StudentProfileDto
{
    public int Id { get; init; }

    public required string Name { get; init; }
    
    public required string Email {  get; init; }
}
