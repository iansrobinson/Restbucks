using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.RulesEngine;

namespace Tests.Restbucks.Client.RulesEngine
{
    [TestFixture]
    public class WhenTests
    {
        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: condition")]
        public void ThrowsExceptionIfConditionIsNull()
        {
            When.IsTrue(null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: contextName")]
        public void ThrowsExceptionIfContextNameIsNull()
        {
            When.IsTrue(() => true).InvokeHandler<DummyResponseHandler>().SetContext(null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException), ExpectedMessage = "Value cannot be empty.\r\nParameter name: contextName")]
        public void ThrowsExceptionIfContextNameIsEmpty()
        {
            When.IsTrue(() => true).InvokeHandler<DummyResponseHandler>().SetContext(string.Empty);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException), ExpectedMessage = "Value cannot be whitespace.\r\nParameter name: contextName")]
        public void ThrowsExceptionIfContextNameIsWhitespace()
        {
            When.IsTrue(() => true).InvokeHandler<DummyResponseHandler>().SetContext(" ");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: createState")]
        public void ThrowsExceptionIfCreateStateFunctionIsNull()
        {
            When.IsTrue(() => true).InvokeHandler<DummyResponseHandler>().SetContext("context-name").ReturnState(null);
        }


        private class DummyResponseHandler : IResponseHandler
        {
            public HandlerResult Handle(HttpResponseMessage response, ApplicationContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
