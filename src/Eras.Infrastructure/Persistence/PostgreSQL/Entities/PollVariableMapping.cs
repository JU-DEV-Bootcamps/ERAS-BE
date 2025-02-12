using Eras.Domain.Common;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
	public class PollVariableMapping : BaseEntity
	{
		public int PollId { get; set; }
		public PollEntity Poll { get; set; } = default!;
		public int VariableId { get; set; }
		public VariableEntity Variable { get; set; } = default!;
		public ICollection<AnswerEntity> Answers { get; set; } = default!;
	}
}