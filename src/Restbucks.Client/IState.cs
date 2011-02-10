namespace Restbucks.Client
{
    public interface IState
    {
        IState Apply(IHttpClientProvider clientProvider);
        ApplicationContext Context { get; }
        bool IsTerminalState { get; }
    }
}