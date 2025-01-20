namespace Eras.Domain.Entities
{
    public class CosmicLatteStatus
    {
        public bool Status { get; set; }
        public DateTime DateTime { get; set; }
        public CosmicLatteStatus(bool Status)
        {
            this.Status = Status;
            this.DateTime = DateTime.Now;
        }
    }
}
