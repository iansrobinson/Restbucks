using System;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.RestToolkit.RulesEngine;
using Rhino.Mocks;
using Tests.RestInPractice.RestToolkit.RulesEngine.Util;

namespace Tests.RestInPractice.RestToolkit.RulesEngine
{
    [TestFixture]
    public class ActionsTests
    {
        [Test]
        public void ShouldReturnSuppliedAction()
        {
            var mockRequestAction = MockRepository.GenerateMock<IRequestAction>();
            mockRequestAction.Expect(a => a.Execute(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities));

            var actions = new Actions(Dummy.ClientCapabilities);
            var action = actions.Do(mockRequestAction);

            action.Execute(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

            mockRequestAction.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnActionThatInvokesSuppliedFunction()
        {
            var expectedResponse = new HttpResponseMessage();

            var actions = new Actions(Dummy.ClientCapabilities);
            var action = actions.Do((r, cl, ct) => expectedResponse);

            Assert.AreEqual(expectedResponse, action.Execute(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: clientCapabilities")]
        public void ThrowsExceptionIfClientCapabilitiesIsNull()
        {
            new Actions(null);
        }
    }
}