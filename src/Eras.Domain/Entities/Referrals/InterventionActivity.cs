namespace Eras.Domain.Entities.Referrals;

public sealed record InterventionActivity
{
    public required string Value { get; init; }

    public static InterventionActivity Create(string value)
    {
        return new InterventionActivity
        {
            Value = DomainNormalization.ToTrimmedOrEmpty(value)
        };
    }

    public override string ToString() => Value;
}
