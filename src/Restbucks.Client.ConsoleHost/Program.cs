using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using log4net.Config;
using Microsoft.Net.Http;
using Restbucks.MediaType;
using Restbucks.NewClient;
using Restbucks.NewClient.RulesEngine;
using Restbucks.NewClient.States;

namespace Restbucks.Client.ConsoleHost
{
    internal class Program
    {
//        private static void Main(string[] args)
//        {
//            XmlConfigurator.Configure();
//
//            var items = new ShopBuilder(null).AddItem(new Item("coffee", new Amount("g", 125))).Build();
//
//            var context = new ApplicationContext();
//            context.Set(ApplicationContextKeys.EntryPointUri, new Uri("http://localhost/restbucks/shop/"));
//            context.Set(new EntityBodyKey(RestbucksMediaType.Value, "http://schemas.restbucks.com/shop", SemanticContext.Rfq), items);
//
//            var responseHandlers = new ResponseHandlers(HttpClientProvider.Instance);
//            
//            var state = new StartedState(null, context);
//            var nextState = state.NextState(responseHandlers);
//
//            while (!nextState.IsTerminalState)
//            {
//                nextState = nextState.NextState(responseHandlers);
//            }
//
//            Console.WriteLine("Finished");
//            Console.ReadLine();
//        }

        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            var items = new ShopBuilder(null).AddItem(new Item("coffee", new Amount("g", 125))).Build();
            var context = new ApplicationContext(
                new KeyValuePair<IKey, object>(new StringKey("home-page-uri"), new Uri("http://localhost/restbucks/shop/")),
                new KeyValuePair<IKey, object>(new EntityBodyKey("request-for-quote", new MediaTypeHeaderValue(RestbucksMediaType.Value), new Uri("http://schemas.restbucks.com/shop")), items));
            var actions = new Actions(ClientCapabilities.Instance);

            var state = new Uninitialized(context);
            Console.WriteLine(state.GetType().Name);
            var nextState = state.NextState(actions);
            Console.WriteLine(nextState.GetType().Name);

            while (!nextState.IsTerminalState)
            {
                nextState = nextState.NextState(actions);
                Console.WriteLine(nextState.GetType().Name);
            }

            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        private class ClientCapabilities : IClientCapabilities
        {
            public static readonly IClientCapabilities Instance = new ClientCapabilities();

            private ClientCapabilities()
            {
            }

            public HttpClient GetHttpClient()
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(RestbucksMediaType.Value));
                return client;
            }

            public IContentFormatter GetContentFormatter(MediaTypeHeaderValue contentType)
            {
                return RestbucksFormatter.Instance;
            }
        }
    }
}