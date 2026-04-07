using FluentValidation;

namespace Eras.Domain.Entities.AssessmentManagement;

public abstract class BaseEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
