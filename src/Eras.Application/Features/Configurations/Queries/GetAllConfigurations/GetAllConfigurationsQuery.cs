using MediatR;

namespace Eras.Application.Features.Configurations.Queries.GetAllConfigurations;
public class GetAllConfigurationsQuery : IRequest<List<Domain.Entities.Configurations>>
{
}
