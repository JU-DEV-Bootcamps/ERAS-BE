using Eras.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Dtos
{
    public class PollDTO
    {
        public int Id { get; set; }
        public string CosmicLatteId { get; set; }
        public DateTime CreatedDate { get; set; } // in our db
        public DateTime ModifiedDate { get; set; }// in our db
        public string PollName { get; set; }
        public PollDTO(int id, string cosmicLatteId, DateTime createdDate, DateTime modifiedDate, string pollName)
        {
            this.Id = id;
            this.CosmicLatteId = cosmicLatteId;
            this.CreatedDate = createdDate;
            this.ModifiedDate = modifiedDate;
            this.PollName = pollName;
        }
    }
}
