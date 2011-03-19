namespace Restbucks.NewClient.RulesEngine
{
    public interface IState
    {
        IState NextState();
        bool IsTerminalState { get; }
    }
}