using MediatR;

namespace Eras.Application.Features.Students.Commands.DeleteStudent;

public sealed record DeleteStudentCommand(int StudentId) : IRequest;
