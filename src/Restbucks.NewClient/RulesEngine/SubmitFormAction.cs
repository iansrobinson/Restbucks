using System.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class SubmitFormAction : IAction
    {
        private readonly FormInfo formInfo;
        private readonly object formData;
        private readonly HttpContentAdapter contentAdapter;
        private readonly HttpClient client;

        public SubmitFormAction(FormInfo formInfo, object formData, HttpContentAdapter contentAdapter, HttpClient client)
        {
            this.formInfo = formInfo;
            this.formData = formData;
            this.contentAdapter = contentAdapter;
            this.client = client;       
        }

        public HttpResponseMessage Execute()
        {
            var request = new HttpRequestMessage
                              {
                                  RequestUri = formInfo.ResourceUri,
                                  Method = formInfo.Method,
                                  Content = contentAdapter.CreateContent(formData, formInfo.ContentType)
                              };

            if (formInfo.Etag != null)
            {
                request.Headers.IfMatch.Add(formInfo.Etag);
            }

            return client.Send(request);
        }
    }
}