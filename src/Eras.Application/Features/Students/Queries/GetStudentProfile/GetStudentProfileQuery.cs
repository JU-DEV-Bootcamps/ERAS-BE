using Eras.Application.DTOs.Student;

using MediatR;

namespace Eras.Application.Features.Students.Queries.GetStudentProfile;

public sealed record GetStudentProfileQuery(int StudentId) : IRequest<StudentRegistrationDto>;
