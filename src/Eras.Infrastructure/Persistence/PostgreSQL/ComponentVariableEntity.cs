using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Eras.Application.Dtos;
using Eras.Domain.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL
{
    public class ComponentVariableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(250)]
        public string Name { get; set; } = string.Empty;

        public int PollId { get; set; }

        [ForeignKey("PollId")]
        public PollsEntity Poll { get; set; }


        public int Position { get; set; }

        public int? ParentId { get; set; }

        // [ForeignKey("ParentId")]
        // public virtual ComponentVariableEntity Parent { get; set; } = default!;


        [DataType(DataType.DateTime)]
        public DateTimeOffset CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTimeOffset ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}