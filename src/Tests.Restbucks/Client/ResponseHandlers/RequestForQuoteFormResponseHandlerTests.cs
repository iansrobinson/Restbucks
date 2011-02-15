using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.Formatters;
using Restbucks.Client.Keys;
using Restbucks.Client.ResponseHandlers;
using Restbucks.MediaType;
using Tests.Restbucks.Client.Helpers;
using Tests.Restbucks.MediaType.Helpers;

namespace Tests.Restbucks.Client.ResponseHandlers
{
    [TestFixture]
    public class RequestForQuoteFormResponseHandlerTests
    {
        private static readonly Shop NeededItems = new Shop(null).AddItem(new Item("coffee", new Amount("g", 125)));
        private const string ExpectedMethod = "put";
        private const string ExpectedContentType = "application/restbucks+xml";

        [Test]
        public void ShouldUseControlDataToSelectNeededItemsFromContextAndSendInRequest()
        {
            var mockEndpoint = new MockEndpoint(CreateRequestForQuoteCreatedResponse());
            var context = CreateContext();

            var responseHandler = new RequestForQuoteFormResponseHandler(new MockEndpointHttpClientProvider(mockEndpoint));
            responseHandler.Handle(CreateRequestForQuoteResponse(), context);

            var requestEntityBody = mockEndpoint.ReceivedRequest.Content.ReadAsObject<Shop>(RestbucksMediaTypeFormatter.Instance);

            Assert.AreEqual(1, requestEntityBody.Items.Count());

            var firstItem = requestEntityBody.Items.First();
            var expectedItem = NeededItems.Items.First();

            Assert.AreEqual(expectedItem.Description, firstItem.Description);
            Assert.AreEqual(expectedItem.Amount.Measure, firstItem.Amount.Measure);
            Assert.AreEqual(expectedItem.Amount.Value, firstItem.Amount.Value);
            Assert.IsNull(firstItem.Cost);
        }

        [Test]
        public void ShouldUseControlDataToSetRequestMethod()
        {
            var mockEndpoint = new MockEndpoint(CreateRequestForQuoteCreatedResponse());
            var context = CreateContext();

            var responseHandler = new RequestForQuoteFormResponseHandler(new MockEndpointHttpClientProvider(mockEndpoint));
            responseHandler.Handle(CreateRequestForQuoteResponse(), context);

            Assert.AreEqual(ExpectedMethod, mockEndpoint.ReceivedRequest.Method.Method);
        }

        [Test]
        public void ShouldUseControlDataToSetContentTypeHeaderOnRequest()
        {
            var mockEndpoint = new MockEndpoint(CreateRequestForQuoteCreatedResponse());
            var context = CreateContext();

            var responseHandler = new RequestForQuoteFormResponseHandler(new MockEndpointHttpClientProvider(mockEndpoint));
            responseHandler.Handle(CreateRequestForQuoteResponse(), context);

            Assert.AreEqual(ExpectedContentType, mockEndpoint.ReceivedRequest.Content.Headers.ContentType.MediaType);
        }

        private static ApplicationContext CreateContext()
        {
            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.ContextName, ContextNames.Rfq);
            context.Set(new EntityBodyKey(RestbucksMediaType.Value, "http://schemas.restbucks.com/shop", ContextNames.Rfq), NeededItems);
            return context;
        }

        private static HttpResponseMessage CreateRequestForQuoteResponse()
        {
            var entityBody = new Shop(new Uri("http://restbucks.com/virtual-directory/"))
                .AddForm(new Form(
                             new Uri("quotes", UriKind.Relative),
                             ExpectedMethod, ExpectedContentType,
                             new Uri("http://schemas.restbucks.com/shop")));

            return CreateResponseMessage(HttpStatusCode.OK, entityBody);
        }

        private static HttpResponseMessage CreateRequestForQuoteCreatedResponse()
        {
            return CreateResponseMessage(HttpStatusCode.Created, new ShopBuilder().Build());
        }

        private static HttpResponseMessage CreateResponseMessage(HttpStatusCode statusCode, Shop entityBody)
        {
            var content = entityBody.ToContent(RestbucksMediaTypeFormatter.Instance);
            content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);

            return new HttpResponseMessage {StatusCode = statusCode, Content = content};
        }
    }
}