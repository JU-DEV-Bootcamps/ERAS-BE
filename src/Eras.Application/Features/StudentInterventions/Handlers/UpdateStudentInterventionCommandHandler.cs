using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.StudentInterventions;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using FluentValidation;

using MediatR;

namespace Eras.Application.Features.StudentInterventions.Handlers;

public sealed class UpdateStudentInterventionCommandHandler(
    IStudentRepository StudentRepository,
    IStudentInterventionEntryRepository EntryRepository,
    IStudentAccessService AccessService,
    ICurrentUserService CurrentUserService,
    IValidator<SaveStudentInterventionDto> Validator,
    IMapper<StudentInterventionEntry, StudentInterventionDto> Mapper)
    : IRequestHandler<UpdateStudentInterventionCommand, StudentInterventionDto>
{
    public async Task<StudentInterventionDto> Handle(
        UpdateStudentInterventionCommand Request,
        CancellationToken CancellationToken)
    {
        _ = await StudentRepository.GetByIdAsync(Request.StudentId)
            ?? throw new NotFoundException($"Student with ID {Request.StudentId} not found.");
        await AccessService.EnsureCanAccessStudentAsync(Request.StudentId, CancellationToken);

        StudentInterventionEntry entry = await EntryRepository.GetByIdAsync(Request.EntryId)
            ?? throw new NotFoundException($"Intervention entry with ID {Request.EntryId} not found.");
        if (entry.StudentId != Request.StudentId)
            throw new NotFoundException($"Intervention entry with ID {Request.EntryId} not found.");

        string sub = CurrentUserService.Sub ?? "System";
        if (!AccessService.IsAdministrator && entry.Audit.CreatedBy != sub)
            throw new BussinessException("You can only change the entries you created.", 403);

        // The kind of an entry never changes: the stored one decides which types are valid.
        SaveStudentInterventionDto changes = new()
        {
            Kind = entry.Kind,
            Type = Request.Entry.Type,
            Description = Request.Entry.Description,
            EntryDate = Request.Entry.EntryDate,
        };
        await StudentInterventionGuards.ValidateAsync(Validator, changes, CancellationToken);

        entry.Type = changes.Type!.Value;
        entry.Description = changes.Description!.Trim();
        entry.EntryDate = changes.EntryDate!.Value;
        entry.Audit.ModifiedBy = sub;
        entry.Audit.ModifiedAt = DateTime.UtcNow;

        StudentInterventionEntry saved = await EntryRepository.UpdateAsync(entry);
        return Mapper.Map(saved);
    }
}
