namespace Restbucks.NewClient.RulesEngine
{
    public interface IState
    {
        IState NextState(Actions actions);
        bool IsTerminalState { get; }
    }
}