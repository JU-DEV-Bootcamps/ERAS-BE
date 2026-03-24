namespace Eras.Domain.Entities.Referrals;

public abstract class BaseEntity<TId>
{
    public required TId Id { get; init; }
}
