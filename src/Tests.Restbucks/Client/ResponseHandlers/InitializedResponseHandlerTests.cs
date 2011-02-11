using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.Formatters;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.States;
using Restbucks.MediaType;
using Tests.Restbucks.Client.Helpers;
using Tests.Restbucks.Client.States.Helpers;
using Tests.Restbucks.MediaType.Helpers;

namespace Tests.Restbucks.Client.ResponseHandlers
{
    [TestFixture]
    public class InitializedResponseHandlerTests
    {
        private static readonly Uri EntryPointUri = new Uri("http://localhost/shop/");
        
        [Test]
        public void WhenContextNameIsEmptyShouldMakeRequestUsingEntryPointUri()
        {
            var response = CreateResponseMessage();
            var mockEndpoint = new MockEndpoint(response);

            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, EntryPointUri);

            var handler = new InitializedResponseHandler();
            handler.Handle(null, context, new MockEndpointHttpClientProvider(mockEndpoint));

            Assert.AreEqual(EntryPointUri, mockEndpoint.ReceivedRequest.RequestUri);
        }

        [Test]
        public void ReturnValueContainsLatestResponse()
        {
            var response = CreateResponseMessage();
            var mockEndpoint = new MockEndpoint(response);

            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, EntryPointUri);

            var handler = new InitializedResponseHandler();
            var result = handler.Handle(null, context, new MockEndpointHttpClientProvider(mockEndpoint));

            Assert.AreEqual(response, result.Response);
        }

        private static HttpResponseMessage CreateResponseMessage()
        {
            var entity = new ShopBuilder().Build();
            var stream = new MemoryStream();

            RestbucksMediaTypeFormatter.Instance.WriteToStream(entity, stream);
            stream.Seek(0, SeekOrigin.Begin);

            var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = content };
        }
    }
}
