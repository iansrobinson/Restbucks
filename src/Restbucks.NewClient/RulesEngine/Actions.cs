using System;
using System.Net.Http;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.NewClient.RulesEngine
{
    public class Actions
    {
        private readonly IClientCapabilities clientCapabilities;

        public Actions(IClientCapabilities clientCapabilities)
        {
            Check.IsNotNull(clientCapabilities, "clientCapabilities");
            this.clientCapabilities = clientCapabilities;
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
            return new ActionObjectInvoker(action, clientCapabilities);
        }

        public IActionInvoker Do(Func<HttpResponseMessage, ApplicationContext, IClientCapabilities, HttpResponseMessage> action)
        {
            return new ActionFunctionInvoker(action, clientCapabilities);
        }

        private class ActionObjectInvoker : IActionInvoker
        {
            private readonly IAction action;
            private readonly IClientCapabilities clientCapabilities;

            public ActionObjectInvoker(IAction action, IClientCapabilities clientCapabilitiesCapabilities)
            {
                this.action = action;
                clientCapabilities = clientCapabilitiesCapabilities;
            }

            public HttpResponseMessage Invoke(HttpResponseMessage previousResponse, ApplicationContext context)
            {
                return action.Execute(previousResponse, context, clientCapabilities);
            }
        }

        private class ActionFunctionInvoker : IActionInvoker
        {
            private readonly Func<HttpResponseMessage, ApplicationContext, IClientCapabilities, HttpResponseMessage> action;
            private readonly IClientCapabilities clientCapabilities;

            public ActionFunctionInvoker(Func<HttpResponseMessage, ApplicationContext, IClientCapabilities, HttpResponseMessage> action, IClientCapabilities clientCapabilities)
            {
                this.action = action;
                this.clientCapabilities = clientCapabilities;
            }

            public HttpResponseMessage Invoke(HttpResponseMessage previousResponse, ApplicationContext context)
            {
                return action(previousResponse, context, clientCapabilities);
            }
        }
    }
}