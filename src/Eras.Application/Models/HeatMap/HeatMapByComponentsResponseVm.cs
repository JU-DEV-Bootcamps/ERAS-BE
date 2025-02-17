namespace Eras.Application.Models.HeatMap
{
    public class HeatMapByComponentsResponseVm
    {
        public IEnumerable<VariableData> Variables { get; set; }
        public IEnumerable<AnswerData> Answers { get; set; }
    }

    public class VariableData
    {
        public string SurveyKind { get; set; }
        public IEnumerable<Variable> Variables { get; set; }
    }

    public class Variable
    {
        public string Description { get; set; }
        public bool IsMultiple { get; set; }
        public IEnumerable<PossibleAnswer> PossibleAnswers { get; set; }
    }
    public class PossibleAnswer
    {
        public string Description { get; set; }
        public int Value { get; set; }
    }

    public class AnswerData
    {
        public string SurveyKind { get; set; }
        public IEnumerable<Answer> Answers { get; set; }
    }
    public class Answer
    {
        public string Description { get; set; }
        public int Value { get; set; }

    }
}
