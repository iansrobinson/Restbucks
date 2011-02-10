using System;
using Restbucks.Client.Http;
using Restbucks.Client.States;

namespace Restbucks.Client.ConsoleHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, new Uri("http://localhost:8080/restbucks/shop/"));

            var state = new StartState(context, null);
            var newState = state.Apply(HttpClientProvider.Instance);
            while (newState != null)
            {
                newState = newState.Apply(HttpClientProvider.Instance);
            }
            Console.ReadLine();
        }
    }
}