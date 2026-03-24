namespace Eras.Domain.Entities.Referrals;

public sealed record Diagnosis
{
    public required string Value { get; init; }

    public static Diagnosis Create(string value)
    {
        return new Diagnosis
        {
            Value = DomainNormalization.ToTrimmedOrEmpty(value)
        };
    }

    public override string ToString() => Value;
}
