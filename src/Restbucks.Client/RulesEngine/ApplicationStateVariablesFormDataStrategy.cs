using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ApplicationServer.Http;

namespace Restbucks.Client.RulesEngine
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
            var content = new ObjectContent(typeof(object), stateVariables.Get<object>(key), new[] { clientCapabilities.GetMediaTypeFormatter(contentType) });         
            content.Headers.ContentType = contentType;

            return content;
        }
    }
}