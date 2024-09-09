namespace Lyt.TranslateRace.Messaging;

public sealed class ResultMessage(Result result )
{
    public Result Result { get; private set; } = result ;
}