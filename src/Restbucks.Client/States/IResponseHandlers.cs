using Restbucks.Client.RulesEngine;

namespace Restbucks.Client.States
{
    public interface IResponseHandlers
    {
        IResponseHandler Get<T>() where T : IResponseHandler;
    }
}