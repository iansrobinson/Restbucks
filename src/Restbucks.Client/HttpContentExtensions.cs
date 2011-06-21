using System;
using System.Linq;
using System.Net.Http;
using Microsoft.ApplicationServer.Http;

namespace Restbucks.Client
{
    public static class HttpContentExtensions
    {
        public static T ReadAsObject<T>(this HttpContent content, params MediaTypeFormatter[] formatters)
        {
            var contentType = content.Headers.ContentType.MediaType;
            var formatter = formatters.FirstOrDefault(f => f.SupportedMediaTypes.Any(m => m.MediaType.Equals(contentType, StringComparison.OrdinalIgnoreCase)));

            return (T) formatter.ReadFromStream(typeof (T), content.ContentReadStream, content.Headers);
        }
    }
}