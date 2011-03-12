using System;
using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class Actions
    {
        private readonly HttpClient client;

        public Actions(HttpClient client)
        {
            this.client = client;
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
            return new ActionObjectInvoker(action, client);
        }

        public IActionInvoker Do(Func<HttpResponseMessage, HttpClient, HttpResponseMessage> action)
        {
            return new ActionFunctionInvoker(action, client);
        }

        private class ActionObjectInvoker : IActionInvoker
        {
            private readonly IAction action;
            private readonly HttpClient client;

            public ActionObjectInvoker(IAction action, HttpClient client)
            {
                this.action = action;
                this.client = client;
            }

            public HttpResponseMessage Invoke(HttpResponseMessage previousResponse)
            {
                return action.Execute(previousResponse, client);
            }
        }

        private class ActionFunctionInvoker : IActionInvoker
        {
            private readonly Func<HttpResponseMessage, HttpClient, HttpResponseMessage> action;
            private readonly HttpClient client;

            public ActionFunctionInvoker(Func<HttpResponseMessage, HttpClient, HttpResponseMessage> action, HttpClient client)
            {
                this.action = action;
                this.client = client;
            }

            public HttpResponseMessage Invoke(HttpResponseMessage previousResponse)
            {
                return action(previousResponse, client);
            }
        }
    }
}