namespace Eras.Domain.Entities.AssessmentManagement;

public sealed class StudentProfile : BaseEntity
{
    public required string StudentCode { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    public string? SupportAndReferralHistory { get; init; }
    public string? CharacterizationOrCurrentContext { get; init; }
}