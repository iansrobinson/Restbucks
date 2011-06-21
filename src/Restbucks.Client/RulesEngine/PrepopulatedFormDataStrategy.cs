using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ApplicationServer.Http;

namespace Restbucks.Client.RulesEngine
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

        public HttpContent CreateFormData(HttpResponseMessage previousResponse, ApplicationStateVariables stateVariables, IClientCapabilities clientCapabilities)
        {
            var content = new ObjectContent(typeof (object), entityBody, new [] {clientCapabilities.GetMediaTypeFormatter(contentType)});
            content.Headers.ContentType = contentType;

            return content;
        }
    }
}