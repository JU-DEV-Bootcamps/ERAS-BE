using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Eras.Infrastructure.Persistence.PostgreSQL
{
    public class PollsEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = string.Empty;


        [DataType(DataType.DateTime)]
        public DateTimeOffset CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTimeOffset ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}