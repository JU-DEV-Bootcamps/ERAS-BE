using Eras.Error.Properties;

namespace Eras.Error.Bussiness;
public class ExistingEvaluationNameException : BussinessException
{
    private readonly string _logMessage;

    public ExistingEvaluationNameException(string EvaluationsName) 
        : base(string.Format(Resources.ExistingEvaluationName, EvaluationsName), 400)
    {
        _logMessage = $"An error occurred creating the evaluation: {EvaluationsName}";
    }

    public override void LogException()
    {
        LogMessage(_logMessage);
        base.LogException();
    }    
}
