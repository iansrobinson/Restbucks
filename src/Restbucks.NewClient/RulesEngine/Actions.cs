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
            return new ActionInvoker(action.Execute, clientCapabilities);
        }

        public IActionInvoker Do(ActionDelegate actionDelegate)
        {
            return new ActionInvoker((response, context, capabilities) => actionDelegate(response, context, capabilities), clientCapabilities);
        }

        private class ActionInvoker : IActionInvoker
        {
            private readonly Func<HttpResponseMessage, ApplicationStateVariables, IClientCapabilities, HttpResponseMessage> action;
            private readonly IClientCapabilities clientCapabilities;

            public ActionInvoker(Func<HttpResponseMessage, ApplicationStateVariables, IClientCapabilities, HttpResponseMessage> action, IClientCapabilities clientCapabilities)
            {
                this.action = action;
                this.clientCapabilities = clientCapabilities;
            }

            public HttpResponseMessage Invoke(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables)
            {
                return action(previousResponse, stateVariables, clientCapabilities);
            }
        }
    }
}