using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class SubmitForm : IAction
    {
        private readonly IFormStrategy formStrategy;
        private readonly HttpContentAdapter contentAdapter;
        private readonly ApplicationContext context;
        private readonly HttpClient client;

        public SubmitForm(IFormStrategy formStrategy, HttpContentAdapter contentAdapter, ApplicationContext context, HttpClient client)
        {
            this.formStrategy = formStrategy;
            this.contentAdapter = contentAdapter;
            this.context = context;
            this.client = client;
        }

        public HttpResponseMessage Execute(HttpResponseMessage previousResponse)
        {
            var formInfo = formStrategy.GetFormInfo(previousResponse, context, contentAdapter);

            var request = new HttpRequestMessage
                              {
                                  RequestUri = formInfo.ResourceUri,
                                  Method = formInfo.Method,
                                  Content = formInfo.FormData
                              };

            if (formInfo.Etag != null)
            {
                request.Headers.IfMatch.Add(formInfo.Etag);
            }

            return client.Send(request);
        }
    }
}