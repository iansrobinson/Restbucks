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

        public HttpResponseMessage Execute(HttpResponseMessage previousResponse, ApplicationContext context, HttpClient client)
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