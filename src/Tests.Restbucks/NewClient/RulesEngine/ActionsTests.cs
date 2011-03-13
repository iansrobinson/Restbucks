﻿using System;
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

            var action = MockRepository.GenerateMock<IAction>();
            action.Expect(a => a.Execute(response, client));

            var actions = new Actions(client);
            var invoker = actions.Do(action);

            invoker.Invoke(response);

            action.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnInvokerThatInvokesSuppliedFunction()
        {
            var newResponse = new HttpResponseMessage();
            var client = new HttpClient();

            var actions = new Actions(client);
            var invoker = actions.Do((r, c) => newResponse);

            Assert.AreEqual(newResponse, invoker.Invoke(new HttpResponseMessage()));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: client")]
        public void ThrowsExceptionIfHttpClientIsNull()
        {
            new Actions(null);
        }
    }
}