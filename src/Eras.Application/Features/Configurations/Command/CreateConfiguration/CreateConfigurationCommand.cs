using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Configurations.Command.CreateConfiguration;
public class CreateConfigurationCommand : IRequest<CreateCommandResponse<Domain.Entities.Configurations>>
{
    public ConfigurationsDTO? Configurations;
}
