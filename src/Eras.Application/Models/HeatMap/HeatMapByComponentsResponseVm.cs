namespace Eras.Application.Models.HeatMap
{
    public class HeatMapByComponentsResponseVm
    {
        public required string ComponentName { get; set; }
        public required VariableData Variables { get; set; }
        public required AnswerData Answers { get; set; }
    }

    public class VariableData
    {
        public IEnumerable<Variable> Variables { get; set; } = new List<Variable>();
    }

    public class Variable
    {
        public required string Description { get; set; }
        public IEnumerable<PossibleAnswer> PossibleAnswers { get; set; } = new List<PossibleAnswer>();
    }
    public class PossibleAnswer
    {
        public required string Description { get; set; }
        public int Value { get; set; }
    }

    public class AnswerData
    {
        public IEnumerable<Answer> Answers { get; set; } = new List<Answer>();
    }
    public class Answer
    {
        public required string Description { get; set; }
        public int Value { get; set; }
    }
}
