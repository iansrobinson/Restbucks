using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class SubmitForm : IAction
    {
        private readonly IFormInfoFactory formInfoFactory;
        private readonly HttpClient client;

        public SubmitForm(IFormInfoFactory formInfoFactory, HttpClient client)
        {
            this.formInfoFactory = formInfoFactory;
            this.client = client;
        }

        public HttpResponseMessage Execute(HttpResponseMessage previousResponse)
        {
            var formInfo = formInfoFactory.CreateFormInfo(previousResponse);

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