using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Net.Http;
using Restbucks.Client.Formatters;
using Restbucks.MediaType;
using Restbucks.RestToolkit.Http;

namespace Restbucks.Client.Adapters
{
    public class HttpResponseMessageToResponse
    {
        private readonly HttpResponseMessage response;

        public HttpResponseMessageToResponse(HttpResponseMessage response)
        {
            this.response = response;
        }

        public Response<Shop> Adapt()
        {
            var headers = new Dictionary<string, IEnumerable<string>>(StringComparer.InvariantCultureIgnoreCase);
            var entityBody = response.Content != null ? response.Content.ReadAsObject<Shop>(new RestbucksMediaTypeFormatter()) : null;

            if (response.Content != null)
            {
                response.Content.Headers.ToList().ForEach(kv => headers.Add(kv.Key, kv.Value)); 
            }
            response.Headers.ToList().ForEach(kv => headers.Add(kv.Key, kv.Value));

            return new Response<Shop>((int) response.StatusCode, headers, entityBody);
        }
    }
}