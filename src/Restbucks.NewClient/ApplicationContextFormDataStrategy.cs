using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using Restbucks.MediaType;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient
{
    public class ApplicationContextFormDataStrategy : IFormDataStrategy
    {
        private readonly EntityBodyKey key;
        private readonly MediaTypeHeaderValue contentType;

        public ApplicationContextFormDataStrategy(EntityBodyKey key, MediaTypeHeaderValue contentType)
        {
            this.key = key;
            this.contentType = contentType;
        }

        public HttpContent CreateFormData(HttpResponseMessage previousResponse, ApplicationContext context, IClientCapabilities clientCapabilities)
        {
            var content = context.Get<Shop>(key).ToContent(RestbucksFormatter.Instance);
            content.Headers.ContentType = contentType;

            return content;
        }
    }
}