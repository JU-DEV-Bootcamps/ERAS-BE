using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Variables.Queries.GetByName;
public class GetVariableByNameQuery: IRequest<GetQueryResponse<Variable>>
{
    public required string VariableName { get; set; }
}
