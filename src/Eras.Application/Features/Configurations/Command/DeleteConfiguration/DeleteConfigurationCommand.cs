using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Models.Response;

using MediatR;

namespace Eras.Application.Features.Configurations.Command.DeleteConfiguration;
public class DeleteConfigurationCommand : IRequest<BaseResponse>
{
    public int ConfigurationId { get; set; }
}
