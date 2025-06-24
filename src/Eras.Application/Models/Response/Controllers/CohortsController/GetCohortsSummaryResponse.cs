using Eras.Domain.Entities;

namespace Eras.Application.Models.Response.Controllers.CohortsController;
public class CohortSummaryResponse
{
    public int CohortCount { get; set; } = 0;
    public int StudentCount { get; set; } = 0;
    public IEnumerable<StudentSummary> Summary { get; set; } = [];
}
