using Eras.Application.DTOs.StudentInterventions;

using MediatR;

namespace Eras.Application.Features.StudentInterventions;

public sealed record GetStudentInterventionsQuery(int StudentId) : IRequest<IReadOnlyList<StudentInterventionDto>>;
