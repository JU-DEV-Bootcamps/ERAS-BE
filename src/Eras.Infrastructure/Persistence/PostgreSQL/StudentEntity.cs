using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Eras.Infrastructure.Persistence.PostgreSQL
{
    [Table("Students")]    
    public class StudentEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Uuid { get; set; }

        [DataType(DataType.DateTime)]
        public DateTimeOffset CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTimeOffset ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}