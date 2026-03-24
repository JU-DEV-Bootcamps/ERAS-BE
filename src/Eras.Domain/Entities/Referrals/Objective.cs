namespace Eras.Domain.Entities.Referrals;

public sealed record Objective
{
    public required string Value { get; init; }

    public static Objective Create(string value)
    {
        return new Objective
        {
            Value = DomainNormalization.ToTrimmedOrEmpty(value)
        };
    }

    public override string ToString() => Value;
}
