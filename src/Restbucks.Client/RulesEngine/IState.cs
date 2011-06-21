namespace Restbucks.NewClient.RulesEngine
{
    public interface IState
    {
        IState NextState(IClientCapabilities clientCapabilities);
        bool IsTerminalState { get; }
    }
}