using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class SubmitFormAction : IAction
    {
        private readonly FormInfo formInfo;
        private readonly HttpContentAdapter contentAdapter;
        private readonly HttpClient client;

        public SubmitFormAction(FormInfo formInfo, HttpContentAdapter contentAdapter, HttpClient client)
        {
            this.formInfo = formInfo;
            this.contentAdapter = contentAdapter;
            this.client = client;       
        }

        public HttpResponseMessage Execute(HttpResponseMessage previousResponse)
        {
            var request = new HttpRequestMessage
                              {
                                  RequestUri = formInfo.ResourceUri,
                                  Method = formInfo.Method,
                                  Content = contentAdapter.CreateContent(formInfo.FormData, formInfo.ContentType)
                              };

            if (formInfo.Etag != null)
            {
                request.Headers.IfMatch.Add(formInfo.Etag);
            }

            return client.Send(request);
        }
    }
}