using Restbucks.Client.RulesEngine;
using Restbucks.Client.States;

namespace Restbucks.Client.ConsoleHost
{
    public class ResponseHandlers : IResponseHandlers
    {
        private readonly IHttpClientProvider clientProvider;

        public ResponseHandlers(IHttpClientProvider clientProvider)
        {
            this.clientProvider = clientProvider;
        }

        public IResponseHandler Get<T>() where T : IResponseHandler
        {
            return (IResponseHandler) typeof (T).GetConstructor(new[] {typeof (IHttpClientProvider)}).Invoke(new object[] {clientProvider});
        }
    }
}