using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.Formatters;
using Restbucks.Client.Http;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.States;
using Restbucks.MediaType;
using Rhino.Mocks;
using Tests.Restbucks.Client.Helpers;
using Tests.Restbucks.Client.States.Helpers;
using Tests.Restbucks.MediaType.Helpers;

namespace Tests.Restbucks.Client.States
{
    [TestFixture]
    public class StartedStateTests
    {
        [Test]
        public void IsNotATerminalState()
        {
            var state = new StartedState(null, new ApplicationContext(), HttpClientProvider.Instance);
            Assert.IsFalse(state.IsTerminalState);
        }

        [Test]
        public void WhenContextNameIsEmptyShouldReturnNewStartState()
        {
            var response = new HttpResponseMessage();
            var mockEndpoint = new MockEndpoint(response);

            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, new Uri("http://localhost/shop"));

            var state = new StartedState(null, context, new MockEndpointHttpClientProvider(mockEndpoint));

            var newState = state.Apply();

            Assert.IsInstanceOf(typeof (StartedState), newState);
            Assert.AreNotEqual(state, newState);
        }

        [Test]
        public void WhenContextNameIsEmptyReturnedStartStateContextNameShouldBeStarted()
        {
            var response = new HttpResponseMessage();
            var mockEndpoint = new MockEndpoint(response);

            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, new Uri("http://localhost/shop"));

            var state = new StartedState(null, context, new MockEndpointHttpClientProvider(mockEndpoint));

            var newState = state.Apply();

            var applicationContext = PrivateField.GetValue<ApplicationContext>("context", newState);
            Assert.AreEqual("started", applicationContext.Get<string>(ApplicationContextKeys.SemanticContext));
        }

        [Test]
        public void WhenContextNameIsEmptyReturnedStartStateShouldContainNewResponse()
        {
            var response = new HttpResponseMessage();
            var mockEndpoint = new MockEndpoint(response);

            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, new Uri("http://localhost/shop"));

            var state = new StartedState(null, context, new MockEndpointHttpClientProvider(mockEndpoint));

            var newState = state.Apply();

            Assert.AreEqual(response, PrivateField.GetValue<HttpResponseMessage>("response", newState));
        }

        private static HttpResponseMessage CreateResponseMessage()
        {
            var entity = new ShopBuilder().Build();
            var stream = new MemoryStream();

            RestbucksMediaTypeFormatter.Instance.WriteToStream(entity, stream);
            stream.Seek(0, SeekOrigin.Begin);

            var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);

            return new HttpResponseMessage {StatusCode = HttpStatusCode.OK, Content = content};
        }

        private class StubResponseHandlerProvider : IResponseHandlerProvider
        {
            private readonly Type handlerType;
            private readonly IResponseHandler handler;

            public StubResponseHandlerProvider(Type handlerType, IResponseHandler handler)
            {
                this.handlerType = handlerType;
                this.handler = handler;
            }

            public IResponseHandler GetFor<T>() where T : IResponseHandler
            {
                if (handlerType.Equals(typeof (T)))
                {
                    return handler;
                }

                throw new ArgumentException(string.Format("Expected type [{0}]. Invoked with type [{1}].", handlerType.Name, typeof (T).Name));
            }
        }

        private class StubResponseHandler : IResponseHandler
        {
            public static readonly IResponseHandler Instance = new StubResponseHandler();
            public static readonly HttpResponseMessage NewResponse = new HttpResponseMessage();

            private StubResponseHandler()
            {
            }

            public HandlerResult Handle(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider)
            {
                return new HandlerResult(true, NewResponse);
            }
        }
    }
}