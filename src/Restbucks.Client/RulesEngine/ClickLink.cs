using System.Net.Http;

namespace Restbucks.Client.RulesEngine
{
    public class ClickLink : IAction
    {
        private readonly ILinkStrategy linkStrategy;

        public ClickLink(ILinkStrategy linkStrategy)
        {
            this.linkStrategy = linkStrategy;
        }

        public HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            var linkInfo = linkStrategy.GetLinkInfo(previousResponse);

            var request = new HttpRequestMessage
                              {
                                  RequestUri = linkInfo.ResourceUri,
                                  Method = HttpMethod.Get
                              };

            return clientCapabilities.GetHttpClient().Send(request);
        }
    }
}