using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Domain.Entities
{
    public class Answer : IBaseEntityData
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ComponentVariableId { get; set; }
        public string AnswerText { get; set; }
        public string Question { get; set; }
        public int Position { get; set; }
        public int? RiskLevel { get; set; }


        public Answer(string AnswerText, string question, int position, int riskLevel, int id, int componentVariableId, DateTime createdDate, DateTime modifiedDate)
        {
            this.AnswerText = AnswerText;
            this.Question = question;
            this.Position = position;
            this.RiskLevel = riskLevel;
            this.Id = id;
            this.ComponentVariableId = componentVariableId;   
            this.CreatedDate = createdDate;
            this.ModifiedDate = modifiedDate;
        }
    }
}
