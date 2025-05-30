using Eras.Application.Models.Response.Controllers.StudentsController;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Students.Queries.GetAll
{
    public sealed record GetAllStudentsQuery(Pagination Query) : IRequest<PagedResult<GetAllStudentsQueryResponse>>;
}
