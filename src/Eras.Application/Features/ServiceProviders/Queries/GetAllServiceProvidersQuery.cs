using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace Eras.Application.Features.ServiceProviders.Queries;
public class GetAllServiceProvidersQuery : IRequest<List<Domain.Entities.ServiceProviders>>
{
}
