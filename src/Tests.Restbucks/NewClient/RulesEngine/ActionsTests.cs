using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.NewClient;
using Restbucks.NewClient.RulesEngine;
using Rhino.Mocks;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class ActionsTests
    {
        private static readonly HttpResponseMessage Response = new HttpResponseMessage();
        private static readonly ApplicationStateVariables StateVariables = new ApplicationStateVariables();
        private static readonly IClientCapabilities DummyClientCapabilities = MockRepository.GenerateStub<IClientCapabilities>();

        [Test]
        public void ShouldReturnSuppliedAction()
        {
            var mockAction = MockRepository.GenerateMock<IAction>();
            mockAction.Expect(a => a.Execute(Response, StateVariables, DummyClientCapabilities));

            var actions = new Actions(DummyClientCapabilities);
            var action = actions.Do(mockAction);

            action.Execute(Response, StateVariables, DummyClientCapabilities);

            mockAction.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnActionThatInvokesSuppliedFunction()
        {
            var expectedResponse = new HttpResponseMessage();

            var actions = new Actions(DummyClientCapabilities);
            var action = actions.Do((r, cl, ct) => expectedResponse);

            Assert.AreEqual(expectedResponse, action.Execute(Response, StateVariables, DummyClientCapabilities));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: clientCapabilities")]
        public void ThrowsExceptionIfClientCapabilitiesIsNull()
        {
            new Actions(null);
        }

        private class ClientCapabilities : IClientCapabilities
        {
            public HttpClient GetHttpClient()
            {
                return new HttpClient();
            }

            public IContentFormatter GetContentFormatter(MediaTypeHeaderValue contentType)
            {
                return RestbucksFormatter.Instance;
            }
        }
    }
}