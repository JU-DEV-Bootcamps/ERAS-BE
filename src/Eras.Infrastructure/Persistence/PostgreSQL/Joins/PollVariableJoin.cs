using Eras.Domain.Common;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Joins
{
	public class PollVariableJoin : BaseEntity, IVersionableEntity
	{
		public int PollId { get; set; }
		public PollEntity Poll { get; set; } = default!;
		public int VariableId { get; set; }
		public VariableEntity Variable { get; set; } = default!;
        public VersionInfo Version { get; set; } = default!;
		public ICollection<AnswerEntity> Answers { get; set; } = default!;
	}
}