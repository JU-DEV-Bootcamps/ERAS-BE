using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Dtos
{
    public class AnswerDTO
    {
        public string Answer { get; set; }
        public string Question { get; set; }
        public int Position { get; set; }
        public double Score { get; set; }

        public AnswerDTO(string Answer, string Question, int Position, double Score)
        {
            this.Answer = Answer;
            this.Question = Question;
            this.Position = Position;
            this.Score = Score;
        }
    }
}
