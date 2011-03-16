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
        [Test]
        public void ShouldReturnInvokerThatInvokesSuppliedAction()
        {
            var response = new HttpResponseMessage();
            var client = new HttpClient();
            var context = new ApplicationContext();

            var mockAction = MockRepository.GenerateMock<IAction>();
            mockAction.Expect(a => a.Execute(response, context, client));

            var actions = new Actions(client);
            var invoker = actions.Do(mockAction);

            invoker.Invoke(response, context);

            mockAction.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnInvokerThatInvokesSuppliedFunction()
        {
            var newResponse = new HttpResponseMessage();
            
            var actions = new Actions(new HttpClient());
            var invoker = actions.Do((r, cl, ct) => newResponse);

            Assert.AreEqual(newResponse, invoker.Invoke(new HttpResponseMessage(), new ApplicationContext()));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: client")]
        public void ThrowsExceptionIfHttpClientIsNull()
        {
            new Actions(null);
        }
    }
}