using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace Eras.Application.Features.Configurations.Queries.GetUserConfigurations;
public class GetUserConfigurationsQuery : IRequest<List<Domain.Entities.Configurations>>
{
    public required string UserId { get; set; }
}
