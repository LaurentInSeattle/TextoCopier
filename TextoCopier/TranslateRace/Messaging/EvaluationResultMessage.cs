namespace Lyt.TranslateRace.Messaging;

public sealed class EvaluationResultMessage(EvaluationResult result )
{
    public EvaluationResult Result { get; private set; } = result ;
}