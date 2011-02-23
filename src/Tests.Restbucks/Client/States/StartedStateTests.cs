using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.Formatters;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.States;
using Restbucks.MediaType;
using Rhino.Mocks;
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
            var state = new StartedState(new ResponseHandlerProvider(), new ApplicationContext(), null);
            Assert.IsFalse(state.IsTerminalState);
        }

        [Test]
        public void WhenContextNameIsEmptyShouldDelegateToInitializedResponseHandler()
        {
            var context = new ApplicationContext();

            var mocks = new MockRepository();
            var handler = mocks.StrictMock<IResponseHandler>();

            using (mocks.Record())
            {
                Expect.Call(handler.Handle(null, context)).Return(new HandlerResult(true, CreateResponseMessage()));
            }
            mocks.Playback();

            var handlerProvider = new StubResponseHandlerProvider(typeof (UninitializedResponseHandler), handler);

            var state = new StartedState(handlerProvider, context, null);
            state.Apply();

            mocks.VerifyAll();
        }

        [Test]
        public void WhenContextNameIsEmptyShouldReturnNewStartState()
        {
            var responseHandlers = new StubResponseHandlerProvider(typeof (UninitializedResponseHandler), StubResponseHandler.Instance);
            var context = new ApplicationContext();
            var state = new StartedState(responseHandlers, context, null);

            var newState = state.Apply();

            Assert.IsInstanceOf(typeof (StartedState), newState);
            Assert.AreNotEqual(state, newState);
        }

        [Test]
        public void WhenContextNameIsEmptyReturnedStartStateContextNameShouldBeStarted()
        {
            var responseHandlers = new StubResponseHandlerProvider(typeof (UninitializedResponseHandler), StubResponseHandler.Instance);
            var context = new ApplicationContext();
            var state = new StartedState(responseHandlers, context, null);

            var newState = state.Apply();

            var applicationContext = PrivateField.GetValue<ApplicationContext>("context", newState);
            Assert.AreEqual("started", applicationContext.Get<string>(ApplicationContextKeys.SemanticContext));
        }

        [Test]
        public void WhenContextNameIsEmptyReturnedStartStateShouldContainNewResponse()
        {
            var responseHandlers = new StubResponseHandlerProvider(typeof (UninitializedResponseHandler), StubResponseHandler.Instance);
            var context = new ApplicationContext();
            var state = new StartedState(responseHandlers, context, null);

            var newState = state.Apply();

            Assert.AreEqual(StubResponseHandler.NewResponse, PrivateField.GetValue<HttpResponseMessage>("response", newState));
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

            public HandlerResult Handle(HttpResponseMessage response, ApplicationContext context)
            {
                return new HandlerResult(true, NewResponse);
            }
        }
    }
}