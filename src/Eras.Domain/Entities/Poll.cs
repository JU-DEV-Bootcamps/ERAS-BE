using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Domain.Entities
{
    public class Poll : IBaseEntityData
    {
        public int Id { get; set; }
        public string CosmicLatteId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // in our db
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;// in our db
        public string PollName { get; set; }

        public Poll(int id, string cosmicLatteId, DateTime createdDate, DateTime modifiedDate, string pollName)
        {
            this.Id = id;
            this.CosmicLatteId = cosmicLatteId;
            this.CreatedDate = createdDate;
            this.ModifiedDate = modifiedDate;
            this.PollName = pollName;
        }
    }
}
