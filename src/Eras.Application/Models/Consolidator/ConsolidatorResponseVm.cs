namespace Eras.Application.Models.Consolidator;

public abstract class ConsolidatorResponseVm
{
    public IEnumerable<Component> Components { get; set; } = [];
}

public class Component
{
    public required string Description { get; set; }
    public IEnumerable<Variable> Variables { get; set; } = new List<Variable>();
}

public abstract class Variable
{
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public int Count { get; set; }
}
