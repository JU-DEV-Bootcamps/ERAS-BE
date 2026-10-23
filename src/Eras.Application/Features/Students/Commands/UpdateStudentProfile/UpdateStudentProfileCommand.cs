using Eras.Application.DTOs.Student;

using MediatR;

namespace Eras.Application.Features.Students.Commands.UpdateStudentProfile;

public sealed record UpdateStudentProfileCommand(int StudentId, StudentRegistrationDto Profile)
    : IRequest<StudentRegistrationDto>;
