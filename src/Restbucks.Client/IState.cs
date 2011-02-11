namespace Restbucks.Client
{
    public interface IState
    {
        IState Apply();
        bool IsTerminalState { get; }
    }
}