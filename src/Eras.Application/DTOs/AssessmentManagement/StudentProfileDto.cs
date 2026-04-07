namespace Eras.Application.DTOs.AssessmentManagement;

public sealed record StudentProfileDto
{
    public Guid? Id { get; init; }

    public required string StudentCode { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    public string? SupportAndReferralHistory { get; init; }
    public string? CharacterizationOrCurrentContext { get; init; }
}
