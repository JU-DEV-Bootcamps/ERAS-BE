using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICosmicLatteService
    {
        Task<CosmicLatteStatus> CosmicApiIsHealthy();
        /*
        Task<ActionResult<Evaluation>> GetEvaluationById(string id);
        Task<List<Evaluation>> GetEvaluations(string name, string startDate, string endDate);
        */
    }
}
