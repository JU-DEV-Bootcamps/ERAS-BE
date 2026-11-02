using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.StudentInterventions;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using MediatR;

namespace Eras.Application.Features.StudentInterventions.Handlers;

public sealed class GetStudentInterventionsQueryHandler(
    IStudentRepository StudentRepository,
    IStudentInterventionEntryRepository EntryRepository,
    IStudentAccessService AccessService,
    IMapper<StudentInterventionEntry, StudentInterventionDto> Mapper)
    : IRequestHandler<GetStudentInterventionsQuery, IReadOnlyList<StudentInterventionDto>>
{
    public async Task<IReadOnlyList<StudentInterventionDto>> Handle(
        GetStudentInterventionsQuery Request,
        CancellationToken CancellationToken)
    {
        _ = await StudentRepository.GetByIdAsync(Request.StudentId)
            ?? throw new NotFoundException($"Student with ID {Request.StudentId} not found.");
        await AccessService.EnsureCanAccessStudentAsync(Request.StudentId, CancellationToken);

        IReadOnlyList<StudentInterventionEntry> entries =
            await EntryRepository.GetByStudentIdAsync(Request.StudentId, CancellationToken);
        return entries.Select(Mapper.Map).ToList();
    }
}
