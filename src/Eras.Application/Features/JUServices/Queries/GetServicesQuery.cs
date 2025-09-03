using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.JUServices.Queries.GetJUServices
{
    public sealed record GetJUServicesQuery() : IRequest<List<JUService>>;
}