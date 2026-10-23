using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;
using Eras.Application.Utils;

using MediatR;

namespace Eras.Application.Features.EvaluationDetails.Queries.GetStudentsRecentAlerts;

public sealed record GetStudentsRecentAlertsQuery(Pagination Query) : IRequest<PagedResult<GetStudentsRecentAlertsResponse>>;
