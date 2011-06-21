namespace Restbucks.RestToolkit.RulesEngine
{
    public interface IState
    {
        IState NextState(IClientCapabilities clientCapabilities);
        bool IsTerminalState { get; }
    }
}