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

            var items = new ShopBuilder(null).AddItem(new Item("coffee", new Amount("g", 125))).Build();

            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, new Uri("http://localhost/restbucks/shop/"));
            context.Set(new EntityBodyKey(RestbucksMediaType.Value, "http://schemas.restbucks.com/shop", SemanticContext.Rfq), items);

            var responseHandlers = new ResponseHandlers(HttpClientProvider.Instance);
            
            var state = new StartedState(null, context);
            var nextState = state.NextState(responseHandlers);

            while (!nextState.IsTerminalState)
            {
                nextState = nextState.NextState(responseHandlers);
            }

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}