using Eras.Application.DTOs.StudentInterventions;

using MediatR;

namespace Eras.Application.Features.StudentInterventions;

public sealed record CreateStudentInterventionCommand(int StudentId, SaveStudentInterventionDto Entry)
    : IRequest<StudentInterventionDto>;

public sealed record UpdateStudentInterventionCommand(int StudentId, int EntryId, SaveStudentInterventionDto Entry)
    : IRequest<StudentInterventionDto>;

public sealed record DeleteStudentInterventionCommand(int StudentId, int EntryId) : IRequest;
