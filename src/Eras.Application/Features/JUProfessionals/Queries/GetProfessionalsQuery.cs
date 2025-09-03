using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Professionals.Queries.GetProfessionals;
public class GetProfessionalsQuery : IRequest<List<JUProfessional>>
{
}
