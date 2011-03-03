using System.Linq;
using System.Net.Http;
using Microsoft.Net.Http;

namespace Restbucks.NewClient
{
    public class SubmitFormAction : IAction
    {
        private readonly FormInfo formInfo;
        private readonly HttpClient client;
        private readonly object formData;
        private readonly IContentFormatter[] formatters;

        public SubmitFormAction(FormInfo formInfo, HttpClient client, object formData, params IContentFormatter[] formatters)
        {
            this.formInfo = formInfo;
            this.client = client;
            this.formData = formData;
            this.formatters = formatters;
        }

        public HttpResponseMessage Execute()
        {
            var formatter = (from f in formatters
                             where f.SupportedMediaTypes.Contains(formInfo.ContentType)
                             select f).FirstOrDefault();

            var content = formData.ToContent(formatter);
            content.Headers.ContentType = formInfo.ContentType;

            var request = new HttpRequestMessage
                                         {
                                             RequestUri = formInfo.ResourceUri,
                                             Method = formInfo.Method,
                                             Content = content
                                         };

            if (formInfo.Etag != null)
            {
                request.Headers.IfMatch.Add(formInfo.Etag);
            }

            return client.Send(request);
        }
    }
}