using System;
using log4net.Config;
using Restbucks.Client.Http;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.States;

namespace Restbucks.Client.ConsoleHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            
            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, new Uri("http://localhost:8080/restbucks/shop/"));

            var responseHandlers = new ResponseHandlerProvider(
                new UninitializedResponseHandler(HttpClientProvider.Instance),
                new StartedResponseHandler(HttpClientProvider.Instance));

            var state = new StartState(responseHandlers, context, null);
            var newState = state.HandleResponse();
            while (newState != null)
            {
                newState = newState.HandleResponse();
            }
            Console.ReadLine();
        }
    }
}