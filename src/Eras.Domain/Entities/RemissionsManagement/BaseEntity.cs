
using FluentValidation;

namespace Eras.Domain.Entities.RemissionsManagement;

public abstract class BaseEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
