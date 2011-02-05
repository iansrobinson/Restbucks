using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;
using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.Client.Adapters;
using Restbucks.Client.Formatters;
using Restbucks.MediaType;
using Tests.Restbucks.MediaType.Helpers;

namespace Tests.Restbucks.Client.Adapters
{
    [TestFixture]
    public class HttpResponseMessageToResponseTests
    {
        [Test]
        public void ShouldAdaptStatusCode()
        {
            var responseMessage = CreateHttpResponseMessage();
            var adapter = new HttpResponseMessageToResponse(responseMessage);
            var response = adapter.Adapt();

            Assert.AreEqual(200, response.StatusCode);
        }

        [Test]
        public void ShouldAdaptContentToEntityBody()
        {
            var baseUriValue = new UniqueId().ToString();
            var responseMessage = CreateHttpResponseMessage(CreateShopContent(baseUriValue));
            var adapter = new HttpResponseMessageToResponse(responseMessage);
            var response = adapter.Adapt();

            Assert.AreEqual(new Uri(baseUriValue), response.EntityBody.BaseUri);
        }

        [Test]
        public void ShouldAdaptContentHeadersToHeaders()
        {
            var responseMessage = CreateHttpResponseMessage();
            var adapter = new HttpResponseMessageToResponse(responseMessage);
            var response = adapter.Adapt();

            Assert.AreEqual(RestbucksMediaType.Value, response.Headers["Content-Type"].First());
        }

        [Test]
        public void ShouldAdaotOtherHeadersToHeaders()
        {
            var responseMessage = CreateHttpResponseMessage();
            var adapter = new HttpResponseMessageToResponse(responseMessage);
            var response = adapter.Adapt();

            Assert.AreEqual("no-cache", response.Headers["Cache-Control"].First());
        }

        [Test]
        public void CreatesNullEntityBodyIfContentIsNull()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK, "OK");
            var adapter = new HttpResponseMessageToResponse(responseMessage);
            var response = adapter.Adapt();

            Assert.IsNull(response.EntityBody);
        }

        [Test]
        public void ShouuldAdaptResponseWithEmptyContentButContentHeaders()
        {
            var responseMessage = CreateHttpResponseMessage(new ByteArrayContent(new byte[] { }));

            var adapter = new HttpResponseMessageToResponse(responseMessage);
            var response = adapter.Adapt();

            Assert.AreEqual(RestbucksMediaType.Value, response.Headers["Content-Type"].First());
        }

        private static HttpResponseMessage CreateHttpResponseMessage(HttpContent content)
        {
            content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);
            var responseMessage = new HttpResponseMessage {StatusCode = HttpStatusCode.OK, Content = content};
            responseMessage.Headers.CacheControl = new CacheControlHeaderValue {NoCache = true};

            return responseMessage;
        }

        private static HttpResponseMessage CreateHttpResponseMessage()
        {
            var content = new ShopBuilder().WithBaseUri(new Uri(new UniqueId().ToString())).Build().ToContent(new RestbucksMediaTypeFormatter());
            return CreateHttpResponseMessage(content);
        }

        private static HttpContent CreateShopContent(string baseUriValue)
        {
            return new ShopBuilder().WithBaseUri(new Uri(baseUriValue)).Build().ToContent(new RestbucksMediaTypeFormatter());
        }
    }
}