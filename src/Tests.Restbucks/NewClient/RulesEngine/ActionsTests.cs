using System;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.NewClient.RulesEngine;
using Rhino.Mocks;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class ActionsTests
    {
        private static readonly HttpResponseMessage Response = new HttpResponseMessage();
        private static readonly IClientCapabilities Client = new ClientCapabilities();
        private static readonly ApplicationContext Context = new ApplicationContext();
        
        [Test]
        public void ShouldReturnInvokerThatInvokesSuppliedAction()
        {
            var mockAction = MockRepository.GenerateMock<IAction>();
            mockAction.Expect(a => a.Execute(Response, Context, Client));

            var actions = new Actions(Client);
            var invoker = actions.Do(mockAction);

            invoker.Invoke(Response, Context);

            mockAction.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnInvokerThatInvokesSuppliedFunction()
        {
            var expectedResponse = new HttpResponseMessage();

            var actions = new Actions(Client);
            var invoker = actions.Do((r, cl, ct) => expectedResponse);

            Assert.AreEqual(expectedResponse, invoker.Invoke(Response, Context));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: clientCapabilities")]
        public void ThrowsExceptionIfClientCapabilitiesIsNull()
        {
            new Actions(null);
        }

        private class ClientCapabilities : IClientCapabilities
        {
            public HttpClient HttpClient
            {
                get { return new HttpClient(); }
            }
        }
    }
}