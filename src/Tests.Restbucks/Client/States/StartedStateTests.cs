using System.Net.Http;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.RulesEngine;
using Restbucks.Client.States;
using Rhino.Mocks;
using Tests.Restbucks.Util;

namespace Tests.Restbucks.Client.States
{
    [TestFixture]
    public class StartedStateTests
    {
        [Test]
        public void IsNotATerminalState()
        {
            var state = new StartedState(null, new ApplicationContext());
            Assert.IsFalse(state.IsTerminalState);
        }

        [Test]
        public void WhenIsUninitializedShouldReturnNewStartState()
        {
            var dummyHandlers = MockRepository.GenerateStub<IResponseHandlers>();
            dummyHandlers.Expect(h => h.Get<UninitializedResponseHandler>()).Return(new StubResponseHandler());

            var state = new StartedState(null, CreateUninitializedContext());

            var newState = state.NextState(dummyHandlers);

            Assert.IsInstanceOf(typeof (StartedState), newState);
            Assert.AreNotEqual(state, newState);
        }

        [Test]
        public void WhenIsUninitializedReturnedStateSemanticContextShouldBeStarted()
        {
            var dummyHandlers = MockRepository.GenerateStub<IResponseHandlers>();
            dummyHandlers.Expect(h => h.Get<UninitializedResponseHandler>()).Return(new StubResponseHandler());

            var state = new StartedState(null, CreateUninitializedContext());

            var newState = state.NextState(dummyHandlers);

            var context = newState.GetPrivateFieldValue<ApplicationContext>("context");
            Assert.AreEqual(SemanticContext.Started, context.Get<string>(ApplicationContextKeys.SemanticContext));
        }

        [Test]
        public void WhenIsUninitializedReturnedStateShouldContainNewResponse()
        {
            var response = new HttpResponseMessage();

            var dummyHandlers = MockRepository.GenerateStub<IResponseHandlers>();
            dummyHandlers.Expect(h => h.Get<UninitializedResponseHandler>()).Return(new StubResponseHandler(response));

            var state = new StartedState(null, CreateUninitializedContext());

            var newState = state.NextState(dummyHandlers);

            Assert.AreEqual(response, newState.GetPrivateFieldValue<HttpResponseMessage>("response"));
        }

        [Test]
        public void WhenIsStartedShouldReturnNewStartState()
        {
            var dummyHandlers = MockRepository.GenerateStub<IResponseHandlers>();
            dummyHandlers.Expect(h => h.Get<StartedResponseHandler>()).Return(new StubResponseHandler());

            var state = new StartedState(new HttpResponseMessage(), CreateStartedContext());

            var newState = state.NextState(dummyHandlers);

            Assert.IsInstanceOf(typeof (StartedState), newState);
            Assert.AreNotEqual(state, newState);
        }

        [Test]
        public void WhenIsStartedReturnedStateSemanticContextShouldBeRfq()
        {
            var dummyHandlers = MockRepository.GenerateStub<IResponseHandlers>();
            dummyHandlers.Expect(h => h.Get<StartedResponseHandler>()).Return(new StubResponseHandler());

            var state = new StartedState(new HttpResponseMessage(), CreateStartedContext());

            var newState = state.NextState(dummyHandlers);

            var context = newState.GetPrivateFieldValue<ApplicationContext>("context");
            Assert.AreEqual(SemanticContext.Rfq, context.Get<string>(ApplicationContextKeys.SemanticContext));
        }

        [Test]
        public void WhenIsStartedReturnedStateShouldContainNewResponse()
        {
            var response = new HttpResponseMessage();

            var dummyHandlers = MockRepository.GenerateStub<IResponseHandlers>();
            dummyHandlers.Expect(h => h.Get<StartedResponseHandler>()).Return(new StubResponseHandler(response));

            var state = new StartedState(new HttpResponseMessage(), CreateStartedContext());

            var newState = state.NextState(dummyHandlers);

            Assert.AreEqual(response, newState.GetPrivateFieldValue<HttpResponseMessage>("response"));
        }

        [Test]
        public void WhenIsRfqShouldReturnNewQuoteRequestedState()
        {
            var dummyHandlers = MockRepository.GenerateStub<IResponseHandlers>();
            dummyHandlers.Expect(h => h.Get<RequestForQuoteFormResponseHandler>()).Return(new StubResponseHandler());

            var state = new StartedState(new HttpResponseMessage(), CreateRfqContext());

            var newState = state.NextState(dummyHandlers);

            Assert.IsInstanceOf(typeof (QuoteRequestedState), newState);
            Assert.AreNotEqual(state, newState);
        }

        [Test]
        public void WhenIsRfqReturnedStateSemanticContextShouldBeEmpty()
        {
            var dummyHandlers = MockRepository.GenerateStub<IResponseHandlers>();
            dummyHandlers.Expect(h => h.Get<RequestForQuoteFormResponseHandler>()).Return(new StubResponseHandler());

            var state = new StartedState(new HttpResponseMessage(), CreateRfqContext());

            var newState = state.NextState(dummyHandlers);

            var context = newState.GetPrivateFieldValue<ApplicationContext>("context");
            Assert.IsFalse(context.ContainsKey(ApplicationContextKeys.SemanticContext));
        }

        [Test]
        public void WhenIsRfqReturnedStateShouldContainNewResponse()
        {
            var response = new HttpResponseMessage();

            var dummyHandlers = MockRepository.GenerateStub<IResponseHandlers>();
            dummyHandlers.Expect(h => h.Get<RequestForQuoteFormResponseHandler>()).Return(new StubResponseHandler(response));

            var state = new StartedState(new HttpResponseMessage(), CreateRfqContext());

            var newState = state.NextState(dummyHandlers);

            Assert.AreEqual(response, newState.GetPrivateFieldValue<HttpResponseMessage>("response"));
        }

        private static ApplicationContext CreateUninitializedContext()
        {
            return new ApplicationContext();
        }

        private static ApplicationContext CreateStartedContext()
        {
            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.SemanticContext, SemanticContext.Started);
            return context;
        }

        private static ApplicationContext CreateRfqContext()
        {
            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.SemanticContext, SemanticContext.Rfq);
            return context;
        }

        private class StubResponseHandler : IResponseHandler
        {
            private readonly HttpResponseMessage newResponse;

            public StubResponseHandler() : this(new HttpResponseMessage())
            {
            }

            public StubResponseHandler(HttpResponseMessage newResponse)
            {
                this.newResponse = newResponse;
            }

            public Result<HttpResponseMessage> Handle(HttpResponseMessage response, ApplicationContext context)
            {
                return new Result<HttpResponseMessage>(true, newResponse);
            }
        }
    }
}