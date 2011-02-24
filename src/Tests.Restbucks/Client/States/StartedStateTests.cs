using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.Formatters;
using Restbucks.Client.Keys;
using Restbucks.Client.RulesEngine;
using Restbucks.Client.States;
using Restbucks.MediaType;
using Tests.Restbucks.Client.Helpers;
using Tests.Restbucks.Client.States.Helpers;

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
            var state = new StartedState(null, CreateUninitializedContext());

            var newState = state.Apply(new StubHttpClientProvider(), new StubResponseHandlers());

            Assert.IsInstanceOf(typeof (StartedState), newState);
            Assert.AreNotEqual(state, newState);
        }

        [Test]
        public void WhenIsUninitializedReturnedStateSemanticContextShouldBeStarted()
        {
            var state = new StartedState(null, CreateUninitializedContext());

            var newState = state.Apply(new StubHttpClientProvider(), new StubResponseHandlers());

            var context = newState.GetPrivateFieldValue<ApplicationContext>("context");
            Assert.AreEqual(SemanticContext.Started, context.Get<string>(ApplicationContextKeys.SemanticContext));
        }

        [Test]
        public void WhenIsUninitializedReturnedStateShouldContainNewResponse()
        {
            var response = new HttpResponseMessage();
            var state = new StartedState(null, CreateUninitializedContext());

            var newState = state.Apply(new StubHttpClientProvider(response), new StubResponseHandlers());

            Assert.AreEqual(response, newState.GetPrivateFieldValue<HttpResponseMessage>("response"));
        }

        [Test]
        public void WhenIsStartedShouldReturnNewStartState()
        {
            var state = new StartedState(CreateEntryPointResponseMessage(), CreateStartedContext());

            var newState = state.Apply(new StubHttpClientProvider(), new StubResponseHandlers());

            Assert.IsInstanceOf(typeof (StartedState), newState);
            Assert.AreNotEqual(state, newState);
        }

        [Test]
        public void WhenIsStartedReturnedStateSemanticContextShouldBeRfq()
        {
            var state = new StartedState(CreateEntryPointResponseMessage(), CreateStartedContext());

            var newState = state.Apply(new StubHttpClientProvider(), new StubResponseHandlers());

            var context = newState.GetPrivateFieldValue<ApplicationContext>("context");
            Assert.AreEqual(SemanticContext.Rfq, context.Get<string>(ApplicationContextKeys.SemanticContext));
        }

        [Test]
        public void WhenIsStartedReturnedStateShouldContainNewResponse()
        {
            var response = new HttpResponseMessage();
            var state = new StartedState(CreateEntryPointResponseMessage(), CreateStartedContext());

            var newState = state.Apply(new StubHttpClientProvider(response), new StubResponseHandlers());

            Assert.AreEqual(response, newState.GetPrivateFieldValue<HttpResponseMessage>("response"));
        }

        [Test]
        public void WhenIsRfqShouldReturnNewQuoteRequestedState()
        {
            var state = new StartedState(CreateRfqResponseMessage(), CreateRfqContext());

            var newState = state.Apply(new StubHttpClientProvider(), new StubResponseHandlers());

            Assert.IsInstanceOf(typeof (QuoteRequestedState), newState);
            Assert.AreNotEqual(state, newState);
        }

        [Test]
        public void WhenIsRfqReturnedStateSemanticContextShouldBeEmpty()
        {
            var state = new StartedState(CreateRfqResponseMessage(), CreateRfqContext());

            var newState = state.Apply(new StubHttpClientProvider(), new StubResponseHandlers());

            var context = newState.GetPrivateFieldValue<ApplicationContext>("context");
            Assert.IsFalse(context.ContainsKey(ApplicationContextKeys.SemanticContext));
        }

        [Test]
        public void WhenIsRfqReturnedStateShouldContainNewResponse()
        {
            var response = new HttpResponseMessage();
            var state = new StartedState(CreateRfqResponseMessage(), CreateRfqContext());

            var newState = state.Apply(new StubHttpClientProvider(response), new StubResponseHandlers());

            Assert.AreEqual(response, newState.GetPrivateFieldValue<HttpResponseMessage>("response"));
        }

        private static ApplicationContext CreateUninitializedContext()
        {
            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, new Uri("http://localhost/shop"));
            return context;
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
            context.Set(
                new EntityBodyKey(RestbucksMediaType.Value, "http://schemas.restbucks.com/shop", SemanticContext.Rfq),
                new Shop(null).AddItem(new Item("coffee", new Amount("g", 100))));
            return context;
        }

        private static HttpResponseMessage CreateEntryPointResponseMessage()
        {
            var entityBody = new Shop(new Uri("http://localhost/"))
                .AddLink(new Link(
                             new Uri("rfq", UriKind.Relative),
                             RestbucksMediaType.Value,
                             new UriLinkRelation(new Uri("http://relations.restbucks.com/rfq"))));
            var content = entityBody.ToContent(RestbucksMediaTypeFormatter.Instance);
            content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);
            return new HttpResponseMessage {Content = content};
        }

        private static HttpResponseMessage CreateRfqResponseMessage()
        {
            var entityBody = new Shop(new Uri("http://localhost/"))
                .AddForm(new Form(
                             new Uri("quotes", UriKind.Relative),
                             "post", "application/restbucks+xml",
                             new Uri("http://schemas.restbucks.com/shop")));
            var content = entityBody.ToContent(RestbucksMediaTypeFormatter.Instance);
            content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);
            return new HttpResponseMessage {Content = content};
        }

        private class StubResponseHandlers : IResponseHandlers
        {
            public IResponseHandler Get<T>() where T : IResponseHandler
            {
                return (IResponseHandler) typeof (T).GetConstructor(new Type[] {}).Invoke(new object[] {});
            }
        }
    }
}