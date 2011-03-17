using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using Restbucks.MediaType;
using Restbucks.NewClient.RulesEngine;

namespace Restbucks.NewClient
{
    public class PrepopulatedFormDataStrategy : IFormDataStrategy
    {
        private readonly Shop entityBody;
        private readonly MediaTypeHeaderValue contentType;

        public PrepopulatedFormDataStrategy(Shop entityBody, MediaTypeHeaderValue contentType)
        {
            this.entityBody = entityBody;
            this.contentType = contentType;
        }

        public HttpContent CreateFormData(HttpResponseMessage previousResponse, ApplicationContext context)
        {
            var content = entityBody.ToContent(RestbucksFormatter.Instance);
            content.Headers.ContentType = contentType;

            return content;
        }
    }
}