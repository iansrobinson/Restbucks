using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.Formatters;
using Restbucks.Client.Http;
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
            var state = new StartedState(null, new ApplicationContext(), HttpClientProvider.Instance);
            Assert.IsFalse(state.IsTerminalState);
        }

        [Test]
        public void WhenIsUninitializedShouldReturnNewStartState()
        {
            var state = new StartedState(null, CreateUninitializedContext(), new StubHttpClientProvider());

            var newState = state.Apply();

            Assert.IsInstanceOf(typeof (StartedState), newState);
            Assert.AreNotEqual(state, newState);
        }

        [Test]
        public void WhenIsUninitializedReturnedStateSemanticContextShouldBeStarted()
        {
            var state = new StartedState(null, CreateUninitializedContext(), new StubHttpClientProvider());

            var newState = state.Apply();

            var context = newState.GetPrivateFieldValue<ApplicationContext>("context");
            Assert.AreEqual(SemanticContext.Started, context.Get<string>(ApplicationContextKeys.SemanticContext));
        }

        [Test]
        public void WhenIsUninitializedReturnedStateShouldContainNewResponse()
        {
            var response = new HttpResponseMessage();
            var state = new StartedState(null, CreateUninitializedContext(), new StubHttpClientProvider(response));

            var newState = state.Apply();

            Assert.AreEqual(response, newState.GetPrivateFieldValue<HttpResponseMessage>("response"));
        }

        [Test]
        public void WhenIsStartedShouldReturnNewStartState()
        {
            var state = new StartedState(CreateEntryPointResponseMessage(), CreateStartedContext(), new StubHttpClientProvider());

            var newState = state.Apply();

            Assert.IsInstanceOf(typeof(StartedState), newState);
            Assert.AreNotEqual(state, newState);
        }

        [Test]
        public void WhenIsStartedReturnedStateSemanticContextShouldBeRfq()
        {
            var state = new StartedState(CreateEntryPointResponseMessage(), CreateStartedContext(), new StubHttpClientProvider());

            var newState = state.Apply();

            var context = newState.GetPrivateFieldValue<ApplicationContext>("context");
            Assert.AreEqual(SemanticContext.Rfq, context.Get<string>(ApplicationContextKeys.SemanticContext));
        }

        [Test]
        public void WhenIsStartedReturnedStateShouldContainNewResponse()
        {
            var response = new HttpResponseMessage();
            var state = new StartedState(CreateEntryPointResponseMessage(), CreateStartedContext(), new StubHttpClientProvider(response));

            var newState = state.Apply();

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
            context.Set(ApplicationContextKeys.EntryPointUri, new Uri("http://localhost/shop"));
            context.Set(ApplicationContextKeys.SemanticContext, SemanticContext.Started);
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
            return new HttpResponseMessage{Content = content};
        }
    }
}