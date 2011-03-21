using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class ApplicationStateVariablesFormDataStrategy : IFormDataStrategy
    {
        private readonly IKey key;
        private readonly MediaTypeHeaderValue contentType;

        public ApplicationStateVariablesFormDataStrategy(IKey key, MediaTypeHeaderValue contentType)
        {
            this.key = key;
            this.contentType = contentType;
        }

        public HttpContent CreateFormData(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            var content = stateVariables.Get<object>(key).ToContent(clientCapabilities.GetContentFormatter(contentType));
            content.Headers.ContentType = contentType;

            return content;
        }
    }
}