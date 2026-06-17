using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Variables.Queries.GetWithNameAndPollId;
public class GetVariablesWithNameAndPollIdQuery: IRequest<GetQueryResponse<List<Variable>>>
{}