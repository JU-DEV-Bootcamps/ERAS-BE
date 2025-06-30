using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using MediatR;

namespace Eras.Application.Features.Configurations.Command.EditConfiguration;
public class EditConfigurationCommand : IRequest<CreateCommandResponse<Domain.Entities.Configurations>>
{
    public ConfigurationsDTO ConfigurationDTO = default!;
}
