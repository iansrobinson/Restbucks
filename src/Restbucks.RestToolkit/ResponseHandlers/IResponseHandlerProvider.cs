namespace Restbucks.Client.ResponseHandlers
{
    public interface IResponseHandlerProvider
    {
        IResponseHandler GetFor<T>() where T : IResponseHandler;
    }
}