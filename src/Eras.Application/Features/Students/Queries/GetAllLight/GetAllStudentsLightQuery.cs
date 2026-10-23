using Eras.Application.DTOs;
using Eras.Application.DTOs.Student;
using MediatR;

namespace Eras.Application.Features.Students.Queries.GetAllLight
{
    public sealed record GetAllStudentsLightQuery() : IRequest<List<StudentLightDto>>;
}