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

        public IGenerateNextRequest ClickLink(ILinkStrategy linkStrategy)
        {
            return new ClickLink(linkStrategy);
        }

        public IGenerateNextRequest SubmitForm(IFormStrategy formStrategy)
        {
            return new SubmitForm(formStrategy);
        }

        public IGenerateNextRequest Do(IGenerateNextRequest generateNextRequest)
        {
            return generateNextRequest;
        }

        public IGenerateNextRequest Do(IssueRequestDelegate issueRequestDelegate)
        {
            return new GenerateNextRequest(issueRequestDelegate);
        }

        private class GenerateNextRequest : IGenerateNextRequest
        {
            private readonly IssueRequestDelegate issueRequestDelegate;

            public GenerateNextRequest(IssueRequestDelegate issueRequestDelegate)
            {
                this.issueRequestDelegate = issueRequestDelegate;
            }

            public HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
            {
                return issueRequestDelegate(previousResponse, stateVariables, clientCapabilities);
            }
        }
    }
}