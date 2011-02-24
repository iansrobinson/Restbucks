using System;
using log4net.Config;
using Restbucks.Client.Http;
using Restbucks.Client.Keys;
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
            context.Set(ApplicationContextKeys.EntryPointUri, new Uri("http://localhost/restbucks/shop/"));
            context.Set(new EntityBodyKey(RestbucksMediaType.Value, "http://schemas.restbucks.com/shop", SemanticContext.Rfq), items);

            var responseHandlers = new ResponseHandlers(HttpClientProvider.Instance);
            
            var state = new StartedState(null, context);
            var newState = state.Apply(responseHandlers);

            while (newState != null && !newState.IsTerminalState)
            {
                newState = newState.Apply(responseHandlers);
            }

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}