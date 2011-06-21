using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using log4net.Config;
using Microsoft.ApplicationServer.Http;
using Restbucks.Client.States;
using Restbucks.MediaType;
using Restbucks.RestToolkit.RulesEngine;

namespace Restbucks.Client.ConsoleHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            var items = new ShopBuilder(null).AddItem(new Item("coffee", new Amount("g", 125))).Build();
            var variables = new ApplicationStateVariables(
                new KeyValuePair<IKey, object>(new StringKey("home-page-uri"), new Uri("http://" + Environment.MachineName + "/restbucks/shop")),
                new KeyValuePair<IKey, object>(new EntityBodyKey("request-for-quote", new MediaTypeHeaderValue(RestbucksMediaType.Value), new Uri("http://schemas.restbucks.com/shop")), items));

            var state = new Uninitialized(variables);
            Console.WriteLine(state.GetType().Name);
            var nextState = state.NextState(ClientCapabilities.Instance);
            Console.WriteLine(nextState.GetType().Name);

            while (!nextState.IsTerminalState)
            {
                nextState = nextState.NextState(ClientCapabilities.Instance);
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

            public MediaTypeFormatter GetMediaTypeFormatter(MediaTypeHeaderValue contentType)
            {
                return RestbucksMediaTypeFormatter.Instance;
            }
        }
    }
}