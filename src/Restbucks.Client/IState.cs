using Restbucks.Client.States;

namespace Restbucks.Client
{
    public interface IState
    {
        IState Apply(IHttpClientProvider clientProvider, IResponseHandlers handlers);
        bool IsTerminalState { get; }
    }
}