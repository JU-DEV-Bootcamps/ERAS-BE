namespace Eras.Application.DTOs.HeatMapDTOs
{
    public class HeatMapComponentsResponseDTO
    {
        public VariableData Variable { get; set; }
        public AnswerData Answers { get; set; }
    }

    public class VariableData
    {
        public List<Variable> variables { get; set; }
        public string SurveyKind { get; set; }
    }

    public class Variable
    {
        public string Description { get; set; }
        public bool IsMultiple { get; set; }
        public List<PossibleAnswer> PossibleAnswers { get; set; }
    }
    public class PossibleAnswer
    {
        public string Description { get; set; }
        public int Value { get; set; }
    }

    public class AnswerData
    {
        public List<Answer> Answers { get; set; }
    }
    public class Answer
    {
        public string Description { get; set; }
        public int Value { get; set; }

    }
}
