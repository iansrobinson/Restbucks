using System;
using System.Net.Http;
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
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: condition")]
        public void ThrowsExceptionIfConditionIsNull()
        {
            When.IsTrue(null).InvokeHandler<DummyResponseHandler>().UpdateContext(c => { }).ReturnState((h,c,r) => new DummyState());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: contextAction")]
        public void ThrowsExceptionIfUpdateContextIsNull()
        {
            When.IsTrue(() => true).InvokeHandler<DummyResponseHandler>().UpdateContext(null).ReturnState((h, c, r) => new DummyState());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: createState")]
        public void ThrowsExceptionIfCreateStateFunctionIsNull()
        {
            When.IsTrue(() => true).InvokeHandler<DummyResponseHandler>().UpdateContext(c => {}).ReturnState(null);
        }

        private class DummyResponseHandler : IResponseHandler
        {
            public HandlerResult Handle(HttpResponseMessage response, ApplicationContext context)
            {
                throw new NotImplementedException();
            }
        }

        private class DummyState : IState
        {
            public IState HandleResponse()
            {
                throw new NotImplementedException();
            }

            public bool IsTerminalState
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}