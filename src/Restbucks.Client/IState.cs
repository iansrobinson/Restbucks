using Restbucks.Client.States;

namespace Restbucks.Client
{
    public interface IState
    {
        IState NextState(IResponseHandlers handlers);
        bool IsTerminalState { get; }
    }
}