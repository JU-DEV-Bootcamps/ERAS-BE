using Eras.Application.Models.Response.Controllers.StudentsController;
using Eras.Application.Utils;

using MediatR;

namespace Eras.Application.Features.Students.Queries.GetAll
{
    public sealed record GetAllStudentsQuery(Pagination Query, string? Search = null) : IRequest<PagedResult<GetAllStudentsQueryResponse>>;
}
