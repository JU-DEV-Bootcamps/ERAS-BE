using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Eras.Infrastructure.Persistence.PostgreSQL
{
    public class AnswersEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ComponentVariableId { get; set; }
        public int RiskLevel { get; set; } // Console.WriteLine(answer.Score);
        public string Question { get; set; } 
        public string AnswerText { get; set; }
        public int Position { get; set; }



        [DataType(DataType.DateTime)]
        public DateTimeOffset CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTimeOffset ModifiedDate { get; set; } = DateTime.UtcNow;
        public double Score { get; set; }

    }
}