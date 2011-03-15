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

            var action = MockRepository.GenerateMock<IAction>();
            action.Expect(a => a.Execute(response, client, context));

            var actions = new Actions(client, context);
            var invoker = actions.Do(action);

            invoker.Invoke(response);

            action.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnInvokerThatInvokesSuppliedFunction()
        {
            var newResponse = new HttpResponseMessage();
            
            var actions = new Actions(new HttpClient(), new ApplicationContext());
            var invoker = actions.Do((r, cl, ct) => newResponse);

            Assert.AreEqual(newResponse, invoker.Invoke(new HttpResponseMessage()));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: client")]
        public void ThrowsExceptionIfHttpClientIsNull()
        {
            new Actions(null, new ApplicationContext());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: context")]
        public void ThrowsExceptionIfContextIsNull()
        {
            new Actions(new HttpClient(), null);
        }
    }
}