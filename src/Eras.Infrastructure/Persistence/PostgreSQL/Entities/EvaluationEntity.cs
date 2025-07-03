using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    using EvStatus = EvaluationConstants.EvaluationStatus;
    public class EvaluationEntity: BaseEntity, IAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CurrentStatus => GetStatus().ToString();
        public string PollName {  get; set; } = string.Empty ;
        public string Country { get; set; } = string.Empty;
        public int ConfigurationId { get; set; }
        public ConfigurationsEntity Configuration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<EvaluationPollJoin> EvaluationPolls { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
        public EvStatus GetStatus()
        {
            if (EvaluationPolls == null || EvaluationPolls.Count == 0)
            {
                return EvStatus.Pending;
            }

            var now = DateTime.UtcNow;
            //TODO: Check for PollInstances (Answers) instead of just the Poll
            return EvaluationPolls.Count != 0 //&& EvaluationPolls.Any(ep => ep.Poll.PollVariables.Any(pv => pv.Answers.Count != 0)))
            ? (now > EndDate
                ? EvStatus.Completed
                : EvStatus.InProgress)
            : (now > EndDate
                ? EvStatus.Uncompleted
                : EvStatus.Ready);
        }
    }
}
