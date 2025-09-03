using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;

using MediatR;

namespace Eras.Application.Features.Configurations.Command.CreateConfiguration;
public class CreateConfigurationCommand : IRequest<CreateCommandResponse<Domain.Entities.Configurations>>
{
    public ConfigurationsDTO? Configurations;
}
