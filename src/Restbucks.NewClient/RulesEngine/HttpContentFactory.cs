using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Microsoft.Net.Http;

namespace Restbucks.NewClient.RulesEngine
{
    public class HttpContentFactory
    {
        private readonly IEnumerable<IContentFormatter> formatters;

        public HttpContentFactory(params IContentFormatter[] formatters)
        {
            if (formatters.Length.Equals(0))
            {
                throw new ArgumentException("Must supply at least one content formatter.", "formatters");
            }
            
            this.formatters = formatters;
        }

        public HttpContent CreateContent(object entityBody, MediaTypeHeaderValue contentType)
        {
            var formatter = (from f in formatters
                             where f.SupportedMediaTypes.Contains(contentType)
                             select f).FirstOrDefault();

            if (formatter == null)
            {
                throw new FormatterNotFoundException(string.Format("Formatter not found for content type '{0}'.", contentType.MediaType));
            }

            var content = entityBody.ToContent(formatter);
            content.Headers.ContentType = contentType;

            return content;
        }
    }
}