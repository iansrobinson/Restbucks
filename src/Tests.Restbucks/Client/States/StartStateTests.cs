using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.Formatters;
using Restbucks.Client.States;
using Restbucks.MediaType;
using Tests.Restbucks.Client.Helpers;
using Tests.Restbucks.Client.States.Helpers;
using Tests.Restbucks.MediaType.Helpers;

namespace Tests.Restbucks.Client.States
{
    [TestFixture]
    public class StartStateTests
    {
        private static readonly Uri EntryPointUri = new Uri("http://localhost/shop/");

        [Test]
        public void IsNotATerminalState()
        {
            var state = new StartState(new ApplicationContext(), null);
            Assert.IsFalse(state.IsTerminalState);
        }

        [Test]
        public void WhenContextNameIsEmptyShouldCallEntryPointUri()
        {
            var response = CreateResponseMessage();
            var mockEndpoint = new MockEndpoint(response);

            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, EntryPointUri);

            var state = new StartState(context, null);
            state.Apply(new MockEndpointHttpClientProvider(mockEndpoint));

            Assert.AreEqual(EntryPointUri, mockEndpoint.ReceivedRequest.RequestUri);
        }

        [Test]
        public void WhenContextNameIsEmptyShouldReturnNewStartState()
        {
            var response = CreateResponseMessage();
            var mockEndpoint = new MockEndpoint(response);

            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, EntryPointUri);

            var state = new StartState(context, null);
            var newState = state.Apply(new MockEndpointHttpClientProvider(mockEndpoint));

            Assert.IsInstanceOf(typeof(StartState), newState);
            Assert.AreNotEqual(state, newState);
        }

        [Test]
        public void WhenContextNameIsEmptyReturnedStartStateShouldContainFetchedResponse()
        {
            var response = CreateResponseMessage();
            var mockEndpoint = new MockEndpoint(response);

            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, EntryPointUri);

            var state = new StartState(context, null);
            var newState = state.Apply(new MockEndpointHttpClientProvider(mockEndpoint));

           Assert.AreEqual(response, PrivateField.GetValue<HttpResponseMessage>("response", newState));
        }

        [Test]
        public void WhenContextNameIsEmptyReturnedStartStateContextNameShouldBeStarted()
        {
            var response = CreateResponseMessage();
            var mockEndpoint = new MockEndpoint(response);

            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, EntryPointUri);

            var state = new StartState(context, null);
            var newState = state.Apply(new MockEndpointHttpClientProvider(mockEndpoint));

            var applicationContext = PrivateField.GetValue<ApplicationContext>("context", newState);
            Assert.AreEqual("started", applicationContext.Get<string>(ApplicationContextKeys.ContextName));
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
    }
}