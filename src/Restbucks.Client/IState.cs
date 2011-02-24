using Restbucks.Client.States;

namespace Restbucks.Client
{
    public interface IState
    {
        IState Apply(IResponseHandlers handlers);
        bool IsTerminalState { get; }
    }
}