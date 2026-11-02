using Eras.Application.DTOs.StudentInterventions;
using Eras.Domain.Entities;

using FluentValidation;

namespace Eras.Application.Validation;

public sealed class SaveStudentInterventionDtoValidator : AbstractValidator<SaveStudentInterventionDto>
{
    public const int DescriptionMaxLength = 1000;

    public SaveStudentInterventionDtoValidator()
    {
        RuleFor(Dto => Dto.Kind)
            .NotNull().WithMessage("Kind is required.");

        RuleFor(Dto => Dto.Type)
            .NotNull().WithMessage("Type is required.");

        RuleFor(Dto => Dto.Type)
            .Must((Dto, Type) => StudentInterventionTypes.IsValidFor(Dto.Kind!.Value, Type!.Value))
            .WithMessage(Dto => $"Type '{Dto.Type}' is not valid for {KindLabel(Dto.Kind)} entries.")
            .When(Dto => Dto.Kind is not null && Dto.Type is not null);

        RuleFor(Dto => Dto.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(DescriptionMaxLength)
            .WithMessage($"Description is too long (maximum {DescriptionMaxLength} characters).");

        RuleFor(Dto => Dto.EntryDate)
            .NotNull().WithMessage("Date is required.");
    }

    private static string KindLabel(StudentInterventionKind? Kind) =>
        Kind == StudentInterventionKind.SupportReceived ? "support received" : "risk identified";
}
