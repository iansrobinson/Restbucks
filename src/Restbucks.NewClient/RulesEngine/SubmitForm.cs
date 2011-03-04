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

            var request = new HttpRequestMessage
                              {
                                  RequestUri = formInfo.ResourceUri,
                                  Method = formInfo.Method,
                                  Content = formInfo.FormData
                              };

            return client.Send(request);
        }
    }
}