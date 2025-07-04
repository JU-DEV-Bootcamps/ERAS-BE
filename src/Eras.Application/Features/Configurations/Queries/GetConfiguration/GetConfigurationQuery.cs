using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace Eras.Application.Features.Configurations.Queries.GetConfiguration;
public class GetConfigurationQuery : IRequest<Domain.Entities.Configurations>
{
    public int ConfigurationId { get; set; }
}
