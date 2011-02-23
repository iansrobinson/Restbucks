using System;
using log4net.Config;
using Restbucks.Client.Http;
using Restbucks.Client.Keys;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.States;
using Restbucks.MediaType;

namespace Restbucks.Client.ConsoleHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            var items = new Shop(null).AddItem(new Item("coffee", new Amount("g", 125)));
            
            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, new Uri("http://" + Environment.MachineName + "/restbucks/shop/"));
            context.Set(new EntityBodyKey(RestbucksMediaType.Value, "http://schemas.restbucks.com/shop", SemanticContext.Rfq), items);

            var responseHandlers = new ResponseHandlerProvider(
                new UninitializedResponseHandler(HttpClientProvider.Instance),
                new StartedResponseHandler(HttpClientProvider.Instance),
                new RequestForQuoteFormResponseHandler(HttpClientProvider.Instance));

            var state = new StartedState(responseHandlers, context, null);
            var newState = state.Apply();

            while (newState != null && !newState.IsTerminalState)
            {
                newState = newState.Apply();
            }

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}