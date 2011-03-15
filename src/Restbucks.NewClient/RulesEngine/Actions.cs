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
            return new ActionObjectInvoker(action, client, context);
        }

        public IActionInvoker Do(Func<HttpResponseMessage, HttpClient, ApplicationContext, HttpResponseMessage> action)
        {
            return new ActionFunctionInvoker(action, client, context);
        }

        private class ActionObjectInvoker : IActionInvoker
        {
            private readonly IAction action;
            private readonly HttpClient client;
            private readonly ApplicationContext context;

            public ActionObjectInvoker(IAction action, HttpClient client, ApplicationContext context)
            {
                this.action = action;
                this.client = client;
                this.context = context;
            }

            public HttpResponseMessage Invoke(HttpResponseMessage previousResponse)
            {
                return action.Execute(previousResponse, client, context);
            }
        }

        private class ActionFunctionInvoker : IActionInvoker
        {
            private readonly Func<HttpResponseMessage, HttpClient, ApplicationContext, HttpResponseMessage> action;
            private readonly HttpClient client;
            private readonly ApplicationContext context;

            public ActionFunctionInvoker(Func<HttpResponseMessage, HttpClient, ApplicationContext, HttpResponseMessage> action, HttpClient client, ApplicationContext context)
            {
                this.action = action;
                this.client = client;
                this.context = context;
            }

            public HttpResponseMessage Invoke(HttpResponseMessage previousResponse)
            {
                return action(previousResponse, client, context);
            }
        }
    }
}