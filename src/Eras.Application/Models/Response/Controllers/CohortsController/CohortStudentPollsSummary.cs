using Eras.Domain.Entities;

namespace Eras.Application.Models.Response.Controllers.CohortsController;
public class CohortStudentPollsSummary
{
    public required Student Student { get; set; }
    public required List<PollInstance> PollInstances { get; set; }
}