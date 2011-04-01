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

        public IAction ClickLink(ILinkStrategy linkStrategy)
        {
            return new ClickLink(linkStrategy);
        }

        public IAction SubmitForm(IFormStrategy formStrategy)
        {
            return new SubmitForm(formStrategy);
        }

        public IAction Do(IAction action)
        {
            return action;
        }

        public IAction Do(ActionDelegate actionDelegate)
        {
            return new Action(actionDelegate);
        }

        private class Action : IAction
        {
            private readonly ActionDelegate actionDelegate;

            public Action(ActionDelegate actionDelegate)
            {
                this.actionDelegate = actionDelegate;
            }

            public HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
            {
                return actionDelegate(previousResponse, stateVariables, clientCapabilities);
            }
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

            public HttpResponseMessage Invoke(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities1)
            {
                return action(previousResponse, stateVariables, clientCapabilities);
            }
        }
    }
}