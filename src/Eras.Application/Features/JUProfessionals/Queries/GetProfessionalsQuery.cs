using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Professionals.Queries.GetProfessionals;

public class GetProfessionalsQuery : IRequest<PagedResult<JUProfessional>>
{
    public Pagination Query { get; set; } = new Pagination();
}
