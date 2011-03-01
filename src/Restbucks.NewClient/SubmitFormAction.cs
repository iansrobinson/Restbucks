using System.Net.Http;

namespace Restbucks.NewClient
{
    public class SubmitFormAction : IAction
    {
        private readonly IFormInfo formInfo;
        private readonly HttpClient client;

        public SubmitFormAction(IFormInfo formInfo, HttpClient client)
        {
            this.formInfo = formInfo;
            this.client = client;
        }

        public HttpResponseMessage Execute()
        {
            var content = new StringContent("content");
            content.Headers.ContentType = formInfo.ContentType;

            return client.Send(new HttpRequestMessage
                                   {
                                       RequestUri = formInfo.ResourceUri,
                                       Method = formInfo.Method,
                                       Content = content
                                   });
        }
    }
}