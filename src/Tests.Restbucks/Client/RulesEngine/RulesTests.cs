using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.RulesEngine;

namespace Tests.Restbucks.Client.RulesEngine
{
    [TestFixture]
    public class RulesTests
    {
        [Test]
        public void ShouldProcessEachRuleInTheOrderItWasAddedToRules()
        {
            var processOrder = new Queue<string>();

            var rules = new Rules(
                new Rule(() =>
                             {
                                 processOrder.Enqueue("first");
                                 return false;
                             }, typeof (FirstResponseHandler), "context-name", (h, c, r) => new DummyState()),
                new Rule(() =>
                             {
                                 processOrder.Enqueue("second");
                                 return false;
                             }, typeof (FirstResponseHandler), "context-name", (h, c, r) => new DummyState()),
                new Rule(() =>
                             {
                                 processOrder.Enqueue("third");
                                 return false;
                             }, typeof (FirstResponseHandler), "context-name", (h, c, r) => new DummyState())
                );

            rules.Evaluate(new ResponseHandlerProvider(), new ApplicationContext(), null);

            Assert.IsTrue(processOrder.SequenceEqual(new[] {"first", "second", "third"}));
        }

        [Test]
        public void ShouldEvaluateRuleIfItIsApplicable()
        {
            var rules = new Rules(
                new Rule(() =>true, typeof(FirstResponseHandler), "context-name", (h, c, r) => new DummyState()));

            var handler = new FirstResponseHandler();
            Assert.IsFalse(handler.WasCalled);

            rules.Evaluate(new ResponseHandlerProvider(handler), new ApplicationContext(), null);

            Assert.IsTrue(handler.WasCalled);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ResponseHandlerMissingException), ExpectedMessage = "Response handler missing. Type: [Tests.Restbucks.Client.RulesEngine.RulesTests+FirstResponseHandler].")]
        public void ShouldThrowExceptionIfResponseHandlerDoesNotExist()
        {
            var rules = new Rules(
                new Rule(() => true, typeof(FirstResponseHandler), "context-name", (h, c, r) => new DummyState()));

            rules.Evaluate(new ResponseHandlerProvider(), new ApplicationContext(), null);
        }

        [Test]
        public void ShouldMoveToNextRuleIfEvaluatingARuleDoesNotSuceed()
        {
        }

        [Test]
        public void ShouldThrowExceptionIfCreateStateFunctionReturnsNull()
        {
        }

        private class FirstResponseHandler : IResponseHandler
        {
            private bool wasCalled;

            public FirstResponseHandler()
            {
                wasCalled = false;
            }

            public HandlerResult Handle(HttpResponseMessage response, ApplicationContext context)
            {
                wasCalled = true;
                return new HandlerResult(true, new HttpResponseMessage());
            }

            public bool WasCalled
            {
                get { return wasCalled; }
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