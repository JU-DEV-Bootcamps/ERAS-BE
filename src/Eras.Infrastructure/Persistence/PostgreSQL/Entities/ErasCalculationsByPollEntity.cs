using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class ErasCalculationsByPollEntity
    {
        public int PollId { get; set; }
        public string PollUuid { get; set; }
        public int ComponentId { get; set; }
        public string ComponentName { get; set; }
        public int PollVariableId { get; set; }
        public string Question { get; set; }
        public string AnswerText { get; set; }
        public int PollInstanceId { get; set; }
        public int PollInstanceRiskSum { get; set; }
        public string Name { get; set; }
        public int RiskSum { get; set; }
        public int RiskCount { get; set; }
        public decimal AverageRisk { get; set; }
        public decimal VariableAverageRisk { get; set; }
        public int AnswerCount { get; set; }
        public decimal Percentage { get; set; }
        public int CohortId { get; set; }
        public string CohortName { get; set; }
        public decimal AverageRiskByCohortComponent { get; set; }
    }
}

