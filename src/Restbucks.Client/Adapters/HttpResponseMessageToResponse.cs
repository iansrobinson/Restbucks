using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Net.Http;
using Restbucks.RestToolkit.Http;

namespace Restbucks.Client.Adapters
{
    public class HttpResponseMessageToResponse<T> : IHttpResponseMessageAdapter<T> where T : class
    {
        private readonly IContentFormatter formatter;

        public HttpResponseMessageToResponse(IContentFormatter formatter)
        {
            this.formatter = formatter;
        }

        public Response<T> Adapt(HttpResponseMessage response)
        {
            IDictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>(StringComparer.InvariantCultureIgnoreCase);
            var entityBody = response.Content != null ? response.Content.ReadAsObject<T>(formatter) : null;

            if (response.Content != null)
            {
                response.Content.Headers.ToList().ForEach(headers.Add);
            }
            response.Headers.ToList().ForEach(kv => headers.Add(kv.Key, kv.Value));

            return new Response<T>((int) response.StatusCode, headers, entityBody);
        }
    }
}