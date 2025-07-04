using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using MediatR;

namespace Eras.Application.Features.ServiceProviders.Command;
public class CreateServiceProviderCommand : IRequest<CreateCommandResponse<Domain.Entities.ServiceProviders>>
{
    public ServiceProvidersDTO? ServiceProviders;
}
