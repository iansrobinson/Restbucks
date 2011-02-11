using System;
using System.Collections.Generic;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.ResponseHandlers;

namespace Tests.Restbucks.Client.ResponseHandlers
{
    [TestFixture]
    public class ResponseHandlerProviderTests
    {
        [Test]
        public void ShouldReturnResponseHandlerByType()
        {
            var provider = new ResponseHandlerProvider(new FirstResponseHandler(), new SecondResponseHandler());
            var handler = provider.GetFor<SecondResponseHandler>();

            Assert.IsInstanceOf(typeof (SecondResponseHandler), handler);
        }

        [Test]
        public void ShouldReturnSameInstanceOfHandlerWithWhichProviderWasInitialized()
        {
            var handler = new SecondResponseHandler();
            var provider = new ResponseHandlerProvider(new FirstResponseHandler(), handler);
            var retrievedHandler = provider.GetFor<SecondResponseHandler>();

            Assert.AreEqual(handler, retrievedHandler);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(KeyNotFoundException))]
        public void ThrowsExceptionIfHandlerDoesNotExistForType()
        {
            var provider = new ResponseHandlerProvider();
            provider.GetFor<FirstResponseHandler>();
        }

        private class FirstResponseHandler : IResponseHandler
        {
            public HandlerResult Handle(HttpResponseMessage response, ApplicationContext context)
            {
                throw new NotImplementedException();
            }
        }

        private class SecondResponseHandler : IResponseHandler
        {
            public HandlerResult Handle(HttpResponseMessage response, ApplicationContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}