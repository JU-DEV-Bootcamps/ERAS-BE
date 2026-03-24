namespace Eras.Domain.Entities.Referrals;

public sealed class InterventionPlan : BaseEntity<InterventionPlanId>
{
    private InterventionPlan()
    {
    }

    public required StudentId StudentId { get; init; }
    public Diagnosis Diagnosis { get; private set; }
    public Objective Objective { get; private set; }
    public string? Notes { get; private set; }

    public static InterventionPlan Create(
        InterventionPlanId id,
        StudentId studentId,
        Diagnosis diagnosis,
        Objective objective,
        string? notes = null)
    {
        return new InterventionPlan
        {
            Id = id,
            StudentId = studentId,
            Diagnosis = diagnosis,
            Objective = objective,
            Notes = DomainNormalization.ToNullableTrimmed(notes)
        };
    }

    public void Update(
        Diagnosis diagnosis,
        Objective objective,
        string? notes)
    {
        Diagnosis = diagnosis;
        Objective = objective;
        Notes = DomainNormalization.ToNullableTrimmed(notes);
    }
}
