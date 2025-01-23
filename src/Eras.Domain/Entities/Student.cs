using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Domain.Entities
{
    public class Student : IBaseEntityData
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Uuid { get; set; }
        public Student(int id, DateTime createdDate, DateTime modifiedDate, string name, string email, string uuid)
        {
            this.Id = id;
            this.CreatedDate = createdDate;
            this.ModifiedDate = modifiedDate;
            this.Name = name;
            this.Email = email;
            this.Uuid = uuid;

        }
    }
}
