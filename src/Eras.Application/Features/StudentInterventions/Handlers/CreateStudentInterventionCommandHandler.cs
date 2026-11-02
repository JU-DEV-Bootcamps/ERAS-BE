using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.StudentInterventions;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using FluentValidation;

using MediatR;

namespace Eras.Application.Features.StudentInterventions.Handlers;

public sealed class CreateStudentInterventionCommandHandler(
    IStudentRepository StudentRepository,
    IStudentInterventionEntryRepository EntryRepository,
    IStudentAccessService AccessService,
    ICurrentUserService CurrentUserService,
    IValidator<SaveStudentInterventionDto> Validator,
    IMapper<StudentInterventionEntry, StudentInterventionDto> Mapper)
    : IRequestHandler<CreateStudentInterventionCommand, StudentInterventionDto>
{
    private const int CreatedByNameMaxLength = 200;

    public async Task<StudentInterventionDto> Handle(
        CreateStudentInterventionCommand Request,
        CancellationToken CancellationToken)
    {
        _ = await StudentRepository.GetByIdAsync(Request.StudentId)
            ?? throw new NotFoundException($"Student with ID {Request.StudentId} not found.");
        await AccessService.EnsureCanAccessStudentAsync(Request.StudentId, CancellationToken);
        await StudentInterventionGuards.ValidateAsync(Validator, Request.Entry, CancellationToken);

        string sub = CurrentUserService.Sub ?? "System";
        string creatorName = CurrentUserService.Name ?? CurrentUserService.Email ?? sub;
        StudentInterventionEntry entry = new()
        {
            StudentId = Request.StudentId,
            Kind = Request.Entry.Kind!.Value,
            Type = Request.Entry.Type!.Value,
            Description = Request.Entry.Description!.Trim(),
            EntryDate = Request.Entry.EntryDate!.Value,
            CreatedByName = creatorName.Length > CreatedByNameMaxLength ? creatorName[..CreatedByNameMaxLength] : creatorName,
            Audit = new AuditInfo { CreatedBy = sub, CreatedAt = DateTime.UtcNow },
        };

        StudentInterventionEntry saved = await EntryRepository.AddAsync(entry);
        return Mapper.Map(saved);
    }
}
