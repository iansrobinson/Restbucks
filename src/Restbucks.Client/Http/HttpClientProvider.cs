using System.Net.Http;
using System.Net.Http.Headers;
using Restbucks.MediaType;

namespace Restbucks.Client.Http
{
    public class HttpClientProvider : IHttpClientProvider
    {
        public static readonly IHttpClientProvider Instance = new HttpClientProvider();

        private HttpClientProvider()
        {
        }

        public HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(RestbucksMediaType.Value));

            return client;
        }
    }
}