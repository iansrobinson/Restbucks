using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace Tests.Restbucks.Client
{
    [TestFixture]
    public class HttpClientTests
    {
        [Test]
        public void RequestResponse()
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK, "OK");
            var tracingResponseChannel = new TracingResponseChannel(expectedResponse);

            var client = new HttpClient{Channel = tracingResponseChannel};
            var response = client.Send(new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost/orders")));
            
            Assert.AreEqual(response, expectedResponse);
            Assert.AreEqual(new Uri("http://localhost/orders"), tracingResponseChannel.Request.RequestUri);
        }
    }

    public class TracingResponseChannel : HttpClientChannel
    {
        private readonly HttpResponseMessage response;
        private HttpRequestMessage request;

        public TracingResponseChannel(HttpResponseMessage response)
        {
            this.response = response;
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.request = request;
            return response;
        }

        public HttpRequestMessage Request
        {
            get { return request; }
        }
    }
}
