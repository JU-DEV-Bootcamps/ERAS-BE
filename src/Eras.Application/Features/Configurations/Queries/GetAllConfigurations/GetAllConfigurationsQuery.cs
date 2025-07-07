using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Configurations.Queries.GetAllConfigurations;
public class GetAllConfigurationsQuery : IRequest<List<Domain.Entities.Configurations>>
{
}
