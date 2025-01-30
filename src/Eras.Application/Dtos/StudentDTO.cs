using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Dtos
{
    public class StudentDTO
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Uuid { get; set; }
        public StudentDTO(int id, DateTime CreatedDate, DateTime ModifiedDate, string name, string Email, string Uuid)
        {
            this.Id = id;
            this.CreatedDate = CreatedDate;
            this.ModifiedDate = ModifiedDate;
            this.Name = name;
            this.Email = Email;
            this.Uuid = Uuid;

        }
    }
}