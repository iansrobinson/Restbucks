using System;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.Http;
using Restbucks.Client.States;
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
            Assert.AreEqual("started", context.Get<string>(ApplicationContextKeys.SemanticContext));
        }

        [Test]
        public void WhenIsUninitializedReturnedStateShouldContainNewResponse()
        {
            var response = new HttpResponseMessage();
            var state = new StartedState(null, CreateUninitializedContext(), new StubHttpClientProvider(response));

            var newState = state.Apply();

            Assert.AreEqual(response, newState.GetPrivateFieldValue<HttpResponseMessage>("response"));
        }

        private static ApplicationContext CreateUninitializedContext()
        {
            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, new Uri("http://localhost/shop"));
            return context;
        }
    }
}