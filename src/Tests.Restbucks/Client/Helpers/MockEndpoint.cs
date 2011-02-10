using System.Net.Http;
using System.Threading;

namespace Tests.Restbucks.Client.Helpers
{
    public class MockEndpoint : HttpClientChannel
    {
        private readonly HttpResponseMessage response;
        private HttpRequestMessage receivedRequest;

        public MockEndpoint(HttpResponseMessage response)
        {
            this.response = response;
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            receivedRequest = request;
            return response;
        }

        public HttpRequestMessage ReceivedRequest
        {
            get { return receivedRequest; }
        }
    }
}