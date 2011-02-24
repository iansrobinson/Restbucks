namespace Restbucks.Client
{
    public interface IState
    {
        IState Apply(IHttpClientProvider clientProvider);
        bool IsTerminalState { get; }
    }
}