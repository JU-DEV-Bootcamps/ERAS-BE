using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.DTOs.AssessmentManagement;

/// <summary>One event of the activity history of an assessment.</summary>
/// <param name="Id">Identifier of the event.</param>
/// <param name="Type">Kind of event.</param>
/// <param name="Description">Text to show, for example "Status changed: Remitted → In Progress".</param>
/// <param name="OccurredAtUtc">When it happened, in UTC.</param>
/// <param name="UserName">Name of the user who acted, as it was at that moment.</param>
public sealed record AssessmentActivityDto(
    int Id,
    AssessmentActivityType Type,
    string Description,
    DateTime OccurredAtUtc,
    string UserName);
