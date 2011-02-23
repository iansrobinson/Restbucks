using System.Net.Http;
using Restbucks.Client.ResponseHandlers;

namespace Restbucks.Client.RulesEngine
{
    public class NullResponseHandler : IResponseHandler
    {
        public static IResponseHandler Instance = new NullResponseHandler();

        private NullResponseHandler()
        {
        }

        public HandlerResult Handle(HttpResponseMessage response, ApplicationContext context)
        {
            return new HandlerResult(true, response);
        }
    }
}