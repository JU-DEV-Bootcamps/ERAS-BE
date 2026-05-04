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

public sealed record GetRemissionByIdQuery(Guid Id) : IRequest<AssessmentDto?>;

public sealed record GetRemissionsByStudentIdQuery(int StudentId) : IRequest<IReadOnlyCollection<AssessmentDto>>;

public sealed record GetRemissionsByStatusQuery(AssessmentStatus Status)
    : IRequest<IReadOnlyCollection<AssessmentDto>>;

public sealed record GetAllRemissionsQuery() : IRequest<IReadOnlyCollection<AssessmentDto>>;