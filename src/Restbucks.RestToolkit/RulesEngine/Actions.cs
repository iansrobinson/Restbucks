using System.Net.Http;
using Restbucks.RestToolkit.Utils;

namespace Restbucks.RestToolkit.RulesEngine
{
    public class Actions
    {
        private readonly IClientCapabilities clientCapabilities;

        public Actions(IClientCapabilities clientCapabilities)
        {
            Check.IsNotNull(clientCapabilities, "clientCapabilities");
            this.clientCapabilities = clientCapabilities;
        }

        public IRequestAction ClickLink(ILinkStrategy linkStrategy)
        {
            return new ClickLink(linkStrategy);
        }

        public IRequestAction SubmitForm(IFormStrategy formStrategy)
        {
            return new SubmitForm(formStrategy);
        }

        public IRequestAction Do(IRequestAction requestAction)
        {
            return requestAction;
        }

        public IRequestAction Do(GenerateNextRequestDelegate generateNextRequestDelegate)
        {
            return new RequestAction(generateNextRequestDelegate);
        }

        private class RequestAction : IRequestAction
        {
            private readonly GenerateNextRequestDelegate generateNextRequestDelegate;

            public RequestAction(GenerateNextRequestDelegate generateNextRequestDelegate)
            {
                this.generateNextRequestDelegate = generateNextRequestDelegate;
            }

            public HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
            {
                return generateNextRequestDelegate(previousResponse, stateVariables, clientCapabilities);
            }
        }
    }
}