using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class PrepopulatedFormDataStrategy : IFormDataStrategy
    {
        private readonly object entityBody;
        private readonly MediaTypeHeaderValue contentType;

        public PrepopulatedFormDataStrategy(object entityBody, MediaTypeHeaderValue contentType)
        {
            this.entityBody = entityBody;
            this.contentType = contentType;
        }

        public HttpContent CreateFormData(HttpResponseMessage previousResponse, ApplicationContext context, IClientCapabilities clientCapabilities)
        {
            var content = entityBody.ToContent(clientCapabilities.GetContentFormatter(contentType));
            content.Headers.ContentType = contentType;

            return content;
        }
    }
}