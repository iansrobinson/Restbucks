using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class ClickLink : IAction
    {
        private readonly ILinkStrategy linkStrategy;

        public ClickLink(ILinkStrategy linkStrategy)
        {
            this.linkStrategy = linkStrategy;
        }

        public HttpResponseMessage Execute(HttpResponseMessage response, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            var linkInfo = linkStrategy.GetLinkInfo(response);

            var request = new HttpRequestMessage
                              {
                                  RequestUri = linkInfo.ResourceUri,
                                  Method = HttpMethod.Get
                              };

            return clientCapabilities.GetHttpClient().Send(request);
        }
    }
}