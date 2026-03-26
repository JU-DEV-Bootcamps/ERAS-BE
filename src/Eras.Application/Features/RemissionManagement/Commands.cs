using Eras.Application.DTOs.RemissionManagement;
using Eras.Domain.Entities.RemissionsManagement;

using MediatR;

namespace Eras.Application.Features.RemissionManagement;

public sealed record CreateStudentProfileCommand(StudentProfileDto StudentProfile) : IRequest<StudentProfileDto>;

public sealed record UpdateStudentProfileCommand(StudentProfileDto StudentProfile) : IRequest<StudentProfileDto>;

public sealed record GetStudentProfileByIdQuery(Guid Id) : IRequest<StudentProfileDto?>;

public sealed record GetStudentProfileByStudentCodeQuery(string StudentCode) : IRequest<StudentProfileDto?>;

public sealed record GetAllStudentProfilesQuery() : IRequest<IReadOnlyCollection<StudentProfileDto>>;

public sealed record CreateRemissionCommand(RemissionDto Remission) : IRequest<RemissionDto>;

public sealed record UpdateRemissionCommand(RemissionDto Remission) : IRequest<RemissionDto>;

public sealed record GetRemissionByIdQuery(Guid Id) : IRequest<RemissionDto?>;

public sealed record GetRemissionsByStudentIdQuery(Guid StudentId) : IRequest<IReadOnlyCollection<RemissionDto>>;

public sealed record GetRemissionsByStatusQuery(RemissionStatus Status)
    : IRequest<IReadOnlyCollection<RemissionDto>>;

public sealed record GetAllRemissionsQuery() : IRequest<IReadOnlyCollection<RemissionDto>>;