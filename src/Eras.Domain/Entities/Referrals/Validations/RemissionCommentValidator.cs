using FluentValidation;

namespace Eras.Domain.Entities.Referrals.Validations;

public sealed class RemissionCommentValidator : AbstractValidator<RemissionComment>
{
    public RemissionCommentValidator()
    {
        RuleFor(x => x.AuthorId).NotNull();
        RuleFor(x => x.CreatedAtUtc).NotEmpty();

        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Comment text is required.");
    }
}
