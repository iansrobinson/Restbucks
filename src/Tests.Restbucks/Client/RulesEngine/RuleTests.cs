using System;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.RulesEngine;

namespace Tests.Restbucks.Client.RulesEngine
{
    [TestFixture]
    public class RuleTests
    {
        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: condition")]
        public void ThrowsExceptionIfConditionIsNull()
        {
            new Rule(null, typeof (DummyResponseHandler), c => { }, (h, c, r) => null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: contextAction")]
        public void ThrowsExceptionIfContextActionIsNull()
        {
            new Rule(()=> true, typeof(DummyResponseHandler), null, (h, c, r) => null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: createState")]
        public void ThrowsExceptionIfCreateStateFunctionIsNull()
        {
            new Rule(() => true, typeof(DummyResponseHandler), c=> { }, null);
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