using FluentValidation;

namespace Eras.Domain.Entities.Referrals.Validations;

public sealed class GroupInterventionParticipantValidator : AbstractValidator<GroupInterventionParticipant>
{
    public GroupInterventionParticipantValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.StudentId).NotNull();
    }
}
