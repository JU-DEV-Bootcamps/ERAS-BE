using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using MediatR;

namespace Eras.Application.Features.StudentInterventions.Handlers;

public sealed class DeleteStudentInterventionCommandHandler(
    IStudentRepository StudentRepository,
    IStudentInterventionEntryRepository EntryRepository,
    IStudentAccessService AccessService,
    ICurrentUserService CurrentUserService)
    : IRequestHandler<DeleteStudentInterventionCommand>
{
    public async Task Handle(DeleteStudentInterventionCommand Request, CancellationToken CancellationToken)
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

        await EntryRepository.SoftDeleteAsync(entry.Id, sub);
    }
}
