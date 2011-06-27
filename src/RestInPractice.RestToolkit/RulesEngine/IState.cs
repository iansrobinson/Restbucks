namespace RestInPractice.RestToolkit.RulesEngine
{
    public interface IState
    {
        IState NextState(IClientCapabilities clientCapabilities);
        bool IsTerminalState { get; }
    }
}