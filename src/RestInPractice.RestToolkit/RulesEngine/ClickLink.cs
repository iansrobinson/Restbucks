using System.Net.Http;

namespace RestInPractice.RestToolkit.RulesEngine
{
    public class ClickLink : IRequestAction
    {
        private readonly ILink link;

        public ClickLink(ILink link)
        {
            this.link = link;
        }

        public HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            var linkInfo = link.GetLinkInfo(previousResponse);

            var request = new HttpRequestMessage
                              {
                                  RequestUri = linkInfo.ResourceUri,
                                  Method = HttpMethod.Get
                              };

            return clientCapabilities.GetHttpClient().Send(request);
        }
    }
}