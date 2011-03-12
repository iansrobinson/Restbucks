using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class ClickLink : IAction
    {
        private readonly ILinkStrategy linkStrategy;
        private readonly HttpClient client;

        public ClickLink(ILinkStrategy linkStrategy, HttpClient client)
        {
            this.linkStrategy = linkStrategy;
            this.client = client;
        }

        public HttpResponseMessage Execute(HttpResponseMessage previousResponse)
        {
            var linkInfo = linkStrategy.GetLinkInfo(previousResponse);

            var request = new HttpRequestMessage
                              {
                                  RequestUri = linkInfo.ResourceUri,
                                  Method = HttpMethod.Get
                              };

            return client.Send(request);
        }
    }
}