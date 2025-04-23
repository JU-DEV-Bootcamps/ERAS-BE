using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class ErasCalculationsByPollEntity
    {
        public string PollUuid { get; set; }
        public string ComponentName { get; set; }
        public int PollVariableId { get; set; }
        public string Question { get; set; }
        public string AnswerText { get; set; }
        public int PollInstanceId { get; set; }
        public string Name { get; set; }
        public int RiskSum { get; set; }
        public int RiskCount { get; set; }
        public decimal AverageRisk { get; set; }
        public decimal VariableAverageRisk { get; set; }
        public int AnswerCount { get; set; }
        public decimal Percentage { get; set; }
    }
}

