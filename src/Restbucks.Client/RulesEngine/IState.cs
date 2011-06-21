namespace Restbucks.Client.RulesEngine
{
    public interface IState
    {
        IState NextState(IClientCapabilities clientCapabilities);
        bool IsTerminalState { get; }
    }
}