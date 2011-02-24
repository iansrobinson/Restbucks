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
        public void WhenContextNameIsEmptyShouldReturnNewStartState()
        {
            var clientProvider = new StubHttpClientProvider(new HttpResponseMessage());

            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, new Uri("http://localhost/shop"));

            var state = new StartedState(null, context, clientProvider);

            var newState = state.Apply();

            Assert.IsInstanceOf(typeof (StartedState), newState);
            Assert.AreNotEqual(state, newState);
        }

        [Test]
        public void WhenContextNameIsEmptyReturnedStartStateContextNameShouldBeStarted()
        {
            var clientProvider = new StubHttpClientProvider(new HttpResponseMessage());

            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, new Uri("http://localhost/shop"));

            var state = new StartedState(null, context, clientProvider);

            var newState = state.Apply();

            var applicationContext = PrivateField.GetValue<ApplicationContext>("context", newState);
            Assert.AreEqual("started", applicationContext.Get<string>(ApplicationContextKeys.SemanticContext));
        }

        [Test]
        public void WhenContextNameIsEmptyReturnedStartStateShouldContainNewResponse()
        {
            var response = new HttpResponseMessage();
            var clientProvider = new StubHttpClientProvider(response);

            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, new Uri("http://localhost/shop"));

            var state = new StartedState(null, context, clientProvider);

            var newState = state.Apply();

            Assert.AreEqual(response, PrivateField.GetValue<HttpResponseMessage>("response", newState));
        }
    }
}