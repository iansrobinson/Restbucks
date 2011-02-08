using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using Restbucks.Client.Adapters;
using Restbucks.RestToolkit.Http;

namespace Restbucks.Client
{
    public class UserAgent : IUserAgent
    {
        private readonly IDictionary<Type, IContentFormatter> formatters;

        public UserAgent(IEnumerable<KeyValuePair<Type, IContentFormatter>> formatters)
        {
            this.formatters = new Dictionary<Type, IContentFormatter>(formatters.Count());
            formatters.ToList().ForEach(this.formatters.Add);
        }

        public Response<T> Invoke<T>(Uri uri, Response<T> previousResponse) where T : class
        {
            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0,0,5);

                var contentFormatter = formatters[typeof(T)];
                var request = new HttpRequestMessage(HttpMethod.Get, uri);

                contentFormatter.SupportedMediaTypes.ToList().ForEach(
                    mt =>
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mt.MediaType)));

                var response = client.Send(request);

                using (response)
                {
                    return new HttpResponseMessageToResponse<T>(contentFormatter).Adapt(response);
                }
            }           
        }
    }
}