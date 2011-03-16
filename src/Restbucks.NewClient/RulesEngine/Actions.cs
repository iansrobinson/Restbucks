using System;
using System.Net.Http;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.NewClient.RulesEngine
{
    public class Actions
    {
        private readonly HttpClient client;
        private readonly ApplicationContext context;

        public Actions(HttpClient client, ApplicationContext context)
        {
            Check.IsNotNull(client, "client");
            Check.IsNotNull(context, "context");

            this.client = client;
            this.context = context;
        }

        public IActionInvoker ClickLink(ILinkStrategy linkStrategy)
        {
            return Do(new ClickLink(linkStrategy));
        }

        public IActionInvoker SubmitForm(IFormStrategy formStrategy)
        {
            return Do(new SubmitForm(formStrategy));
        }

        public IActionInvoker Do(IAction action)
        {
            return new ActionObjectInvoker(action, context, client);
        }

        public IActionInvoker Do(Func<HttpResponseMessage, ApplicationContext, HttpClient, HttpResponseMessage> action)
        {
            return new ActionFunctionInvoker(action, context, client);
        }

        private class ActionObjectInvoker : IActionInvoker
        {
            private readonly IAction action;
            private readonly ApplicationContext context;
            private readonly HttpClient client;

            public ActionObjectInvoker(IAction action, ApplicationContext context, HttpClient client)
            {
                this.action = action;
                this.context = context;
                this.client = client;
            }

            public HttpResponseMessage Invoke(HttpResponseMessage previousResponse)
            {
                return action.Execute(previousResponse, context, client);
            }
        }

        private class ActionFunctionInvoker : IActionInvoker
        {
            private readonly Func<HttpResponseMessage, ApplicationContext, HttpClient, HttpResponseMessage> action;
            private readonly HttpClient client;
            private readonly ApplicationContext context;

            public ActionFunctionInvoker(Func<HttpResponseMessage, ApplicationContext, HttpClient, HttpResponseMessage> action, ApplicationContext context, HttpClient client)
            {
                this.action = action;
                this.client = client;
                this.context = context;
            }

            public HttpResponseMessage Invoke(HttpResponseMessage previousResponse)
            {
                return action(previousResponse, context, client);
            }
        }
    }
}