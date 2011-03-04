using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class HttpContentFactory
    {
        private readonly IEnumerable<IContentFormatter> formatters;

        public HttpContentFactory(params IContentFormatter[] formatters)
        {
            this.formatters = formatters;
        }

        public HttpContent CreateContent(object entityBody, MediaTypeHeaderValue contentType)
        {
            var formatter = (from f in formatters
                             where f.SupportedMediaTypes.Contains(contentType)
                             select f).FirstOrDefault();

            var content = entityBody.ToContent(formatter);
            content.Headers.ContentType = contentType;

            return content;
        }
    }
}