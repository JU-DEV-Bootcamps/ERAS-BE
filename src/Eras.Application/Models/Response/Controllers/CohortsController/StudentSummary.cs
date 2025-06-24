namespace Eras.Application.Models.Response.Controllers.CohortsController;
public class StudentSummary
{
    public required string StudentUuid { get; set; } = string.Empty;
    public required string StudentName { get; set; } = string.Empty;
    public int? CohortId { get; set; }
    public string? CohortName { get; set; }
    public required decimal PollinstancesAverage { get; set; }
    public required int PollinstancesCount { get; set; }
}
