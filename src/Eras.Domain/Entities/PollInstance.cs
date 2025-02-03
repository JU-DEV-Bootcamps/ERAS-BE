namespace Eras.Domain.Entities
{
    public class PollInstance : BaseEntity
    {
        public ICollection<Answer> Answers { get; set; } = [];
    }
}