using FluentValidation;

namespace Eras.Domain.Entities.Referrals.Validations;

public sealed class InterventionAttachmentValidator : AbstractValidator<InterventionAttachment>
{
    public InterventionAttachmentValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.UploadedBy).NotNull();
        RuleFor(x => x.UploadedAtUtc).NotEmpty();

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("The file name is required.");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("The content type is required.");

        RuleFor(x => x.StoragePath)
            .NotEmpty()
            .WithMessage("The storage path is required.");
    }
}
