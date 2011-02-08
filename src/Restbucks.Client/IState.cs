namespace Restbucks.Client
{
    public interface IState
    {
        IState Execute(IUserAgent userAgent);
        ApplicationContext Context { get; }
        bool IsTerminalState { get; }
    }
}