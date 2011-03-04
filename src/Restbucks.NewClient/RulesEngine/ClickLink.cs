using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class ClickLink : IAction
    {
        private readonly ILinkInfoFactory linkInfoFactory;
        private readonly HttpClient client;

        public ClickLink(ILinkInfoFactory linkInfoFactory, HttpClient client)
        {
            this.linkInfoFactory = linkInfoFactory;
            this.client = client;
        }

        public HttpResponseMessage Execute(HttpResponseMessage previousResponse)
        {
            var linkInfo = linkInfoFactory.CreateLinkInfo(previousResponse);

            var request = new HttpRequestMessage
                              {
                                  RequestUri = linkInfo.ResourceUri,
                                  Method = HttpMethod.Get
                              };

            return client.Send(request);
        }
    }
}