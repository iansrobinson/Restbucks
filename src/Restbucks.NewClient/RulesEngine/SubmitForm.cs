using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class SubmitForm : IActionInvoker
    {
        private readonly IFormStrategy formStrategy;
        private readonly HttpClient client;

        public SubmitForm(IFormStrategy formStrategy, HttpClient client)
        {
            this.formStrategy = formStrategy;
            this.client = client;
        }

        public HttpResponseMessage Invoke(HttpResponseMessage previousResponse)
        {
            var formInfo = formStrategy.GetFormInfo(previousResponse);
            
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = formInfo.ContentType;

            var request = new HttpRequestMessage
                              {
                                  RequestUri = formInfo.ResourceUri,
                                  Method = formInfo.Method,
                                  Content = content
                              };

            return client.Send(request);
        }
    }
}