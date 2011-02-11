namespace Restbucks.Client
{
    public interface IState
    {
        IState HandleResponse();
        bool IsTerminalState { get; }
    }
}