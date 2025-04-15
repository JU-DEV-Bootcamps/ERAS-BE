using Eras.Domain.Entities;

namespace Eras.Application.Models.Consolidator;
public class TopConsolidatorResponseVm : ConsolidatorResponseVm
{
    public IEnumerable<TopVariable> Variables { get; set; } = [];

    public class TopVariable : ReportVariable
    {
        public double TopRisk { get; set; }
        public List<Student> StudentsAtTopRisk { get; set; } = [];
    }
}
