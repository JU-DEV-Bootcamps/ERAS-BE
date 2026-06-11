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
        public string PollName {  get; set; } = string.Empty ;
        public string Country { get; set; } = string.Empty;
        public int ConfigurationId { get; set; }
        public ConfigurationsEntity Configuration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<EvaluationPollJoin> EvaluationPolls { get; set; } = [];
        public ICollection<PollInstanceEntity> PollInstances { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
    }
}
