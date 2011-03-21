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

        public IActionInvoker Do(ExecuteAction executeAction)
        {
            return new ActionFunctionInvoker(executeAction, clientCapabilities);
        }

        private class ActionObjectInvoker : IActionInvoker
        {
            private readonly IAction action;
            private readonly IClientCapabilities clientCapabilities;

            public ActionObjectInvoker(IAction action, IClientCapabilities clientCapabilities)
            {
                this.action = action;
                this.clientCapabilities = clientCapabilities;
            }

            public HttpResponseMessage Invoke(HttpResponseMessage previousResponse, ApplicationContext context)
            {
                return action.Execute(previousResponse, context, clientCapabilities);
            }
        }

        private class ActionFunctionInvoker : IActionInvoker
        {
            private readonly ExecuteAction executeAction;
            private readonly IClientCapabilities clientCapabilities;

            public ActionFunctionInvoker(ExecuteAction executeAction, IClientCapabilities clientCapabilities)
            {
                this.executeAction = executeAction;
                this.clientCapabilities = clientCapabilities;
            }

            public HttpResponseMessage Invoke(HttpResponseMessage previousResponse, ApplicationContext context)
            {
                return executeAction(previousResponse, context, clientCapabilities);
            }
        }
    }
}