using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using Tests.Restbucks.Client.Helpers;

namespace Tests.Restbucks.Client
{
    [TestFixture]
    public class HttpClientTests
    {
        [Test]
        public void RequestResponse()
        {
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK, "OK");
            var mockEndpoint = new MockEndpoint(expectedResponse);

            var client = new HttpClient {Channel = mockEndpoint};
            var response = client.Send(new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost/orders")));

            Assert.AreEqual(response, expectedResponse);
            Assert.AreEqual(new Uri("http://localhost/orders"), mockEndpoint.ReceivedRequest.RequestUri);
        }
    }
}