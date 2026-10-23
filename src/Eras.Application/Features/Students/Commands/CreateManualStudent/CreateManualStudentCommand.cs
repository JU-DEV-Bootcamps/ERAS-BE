using Eras.Application.DTOs.Student;

using MediatR;

namespace Eras.Application.Features.Students.Commands.CreateManualStudent;

public sealed record CreateManualStudentCommand(StudentRegistrationDto Profile) : IRequest<StudentRegistrationDto>;
