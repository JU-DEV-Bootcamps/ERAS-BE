using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement;

public sealed record CreateStudentProfileCommand(StudentProfileDto StudentProfile) : IRequest<StudentProfileDto>;

public sealed record UpdateStudentProfileCommand(StudentProfileDto StudentProfile) : IRequest<StudentProfileDto>;

public sealed record GetStudentProfileByIdQuery(Guid Id) : IRequest<StudentProfileDto?>;

public sealed record GetStudentProfileByStudentCodeQuery(string StudentCode) : IRequest<StudentProfileDto?>;

public sealed record GetAllStudentProfilesQuery() : IRequest<IReadOnlyCollection<StudentProfileDto>>;

public sealed record CreateRemissionCommand(AssessmentDto Remission) : IRequest<AssessmentDto>;

public sealed record UpdateRemissionCommand(AssessmentDto Remission) : IRequest<AssessmentDto>;

public sealed record GetRemissionByIdQuery(int Id) : IRequest<AssessmentDto?>;

public sealed record GetRemissionsByStudentIdQuery(int StudentId) : IRequest<IReadOnlyCollection<AssessmentDto>>;

public sealed record GetRemissionsByStatusQuery(AssessmentStatus Status)
    : IRequest<IReadOnlyCollection<AssessmentDto>>;

public sealed record GetAllRemissionsQuery() : IRequest<IReadOnlyCollection<AssessmentDto>>;

public sealed record UpsertInterventionsCommand(int AssessmentId, IReadOnlyCollection<InterventionDto> Interventions)
    : IRequest<IReadOnlyCollection<InterventionDto>>;

public sealed record GetInterventionsByAssessmentQuery(int AssessmentId)
    : IRequest<IReadOnlyCollection<InterventionDto>>;

public sealed record AddInterventionCommand(int AssessmentId, InterventionDto Intervention)
    : IRequest<InterventionDto>;

public sealed record DeleteInterventionCommand(int AssessmentId, int InterventionId)
    : IRequest;