namespace Restbucks.Client
{
    public interface IState
    {
        IState Apply(IUserAgent userAgent);
        ApplicationContext Context { get; }
        bool IsTerminalState { get; }
    }
}