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
        [Test]
        public void ShouldUseFormControlDataToSelectNeededItemsFromContext()
        {
            var mockEndpoint = new MockEndpoint(CreateRequestForQuoteCreatedResponse());

            var neededItems = new Shop(null).AddItem(new Item("coffee", new Amount("g", 125)));
            var context = new ApplicationContext();
            var response = CreateRequestForQuoteResponse();

            context.Set(ApplicationContextKeys.ContextName, ContextNames.Rfq);
            context.Set(new EntityBodyKey(RestbucksMediaType.Value, "http://schemas.restbucks.com/shop.xsd", ContextNames.Rfq), neededItems);

            var responseHandler = new RequestForQuoteFormResponseHandler(new MockEndpointHttpClientProvider(mockEndpoint));
            responseHandler.Handle(response, context);

            var requestEntityBody = mockEndpoint.ReceivedRequest.Content.ReadAsObject<Shop>(RestbucksMediaTypeFormatter.Instance);

            Assert.AreEqual(1, requestEntityBody.Items.Count());
                        
            var firstItem = requestEntityBody.Items.First();
            Assert.AreEqual("coffee", firstItem.Description);
            Assert.AreEqual("g", firstItem.Amount.Measure);
            Assert.AreEqual(125, firstItem.Amount.Value);
            Assert.IsNull(firstItem.Cost);
        }

        private static HttpResponseMessage CreateRequestForQuoteResponse()
        {
            var entityBody = new Shop(new Uri("http://restbucks.com/virtual-directory/"))
                .AddForm(new Form(
                             new Uri("quotes", UriKind.Relative),
                             "post", "application/restbucks+xml",
                             new Uri("http://schemas.restbucks.com/shop.xsd")));

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