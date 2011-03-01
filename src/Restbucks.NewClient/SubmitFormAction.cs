using System.Linq;
using System.Net.Http;
using Microsoft.Net.Http;

namespace Restbucks.NewClient
{
    public class SubmitFormAction : IAction
    {
        private readonly IFormInfo formInfo;
        private readonly HttpClient client;
        private readonly object formData;
        private readonly IContentFormatter[] formatters;

        public SubmitFormAction(IFormInfo formInfo, HttpClient client, object formData, params IContentFormatter[] formatters)
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

            return client.Send(new HttpRequestMessage
                                   {
                                       RequestUri = formInfo.ResourceUri,
                                       Method = formInfo.Method,
                                       Content = content
                                   });
        }
    }
}