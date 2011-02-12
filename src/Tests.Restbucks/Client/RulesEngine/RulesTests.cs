using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
                             }, typeof (SuccessfulResponseHandler), 
                             c => c.Set(ApplicationContextKeys.ContextName, "context-name"), 
                             (h, c, r) => new DummyState()),
                new Rule(() =>
                             {
                                 processOrder.Enqueue("second");
                                 return false;
                             }, typeof (SuccessfulResponseHandler), 
                             c => c.Set(ApplicationContextKeys.ContextName, "context-name"), 
                             (h, c, r) => new DummyState()),
                new Rule(() =>
                             {
                                 processOrder.Enqueue("third");
                                 return false;
                             }, typeof (SuccessfulResponseHandler), 
                             c => c.Set(ApplicationContextKeys.ContextName, "context-name"), 
                             (h, c, r) => new DummyState())
                );

            rules.Evaluate(new ResponseHandlerProvider(), new ApplicationContext(), null);

            Assert.IsTrue(processOrder.SequenceEqual(new[] {"first", "second", "third"}));
        }

        [Test]
        public void ShouldOnlyEvaluateRuleIfItIsApplicable()
        {
            var rules = new Rules(
                new Rule(() => true, typeof (SuccessfulResponseHandler), c => c.Set(ApplicationContextKeys.ContextName, "context-name"), (h, c, r) => new DummyState()));

            var handler = new SuccessfulResponseHandler();
            Assert.IsFalse(handler.WasCalled);

            rules.Evaluate(new ResponseHandlerProvider(handler), new ApplicationContext(), null);

            Assert.IsTrue(handler.WasCalled);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ResponseHandlerMissingException), ExpectedMessage = "Response handler missing. Type: [Tests.Restbucks.Client.RulesEngine.RulesTests+SuccessfulResponseHandler].")]
        public void ShouldThrowExceptionIfResponseHandlerDoesNotExist()
        {
            var rules = new Rules(
                new Rule(() => true, typeof (SuccessfulResponseHandler), c => c.Set(ApplicationContextKeys.ContextName, "context-name"), (h, c, r) => new DummyState()));

            rules.Evaluate(new ResponseHandlerProvider(), new ApplicationContext(), null);
        }

        [Test]
        public void ShouldMoveToNextRuleIfEvaluatingARuleDoesNotSucceed()
        {
            var rules = new Rules(
                new Rule(() => true, typeof (UnsuccessfulResponseHandler), c => c.Set(ApplicationContextKeys.ContextName, "context-name"), (h, c, r) => new DummyState()),
                new Rule(() => true, typeof (SuccessfulResponseHandler), c => c.Set(ApplicationContextKeys.ContextName, "context-name"), (h, c, r) => new DummyState()));

            var firstHandler = new UnsuccessfulResponseHandler();
            var secondHandler = new SuccessfulResponseHandler();

            rules.Evaluate(new ResponseHandlerProvider(firstHandler, secondHandler), new ApplicationContext(), null);

            Assert.IsTrue(firstHandler.WasCalled);
            Assert.IsTrue(secondHandler.WasCalled);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (NullStateException))]
        public void ShouldThrowExceptionIfCreateStateFunctionReturnsNull()
        {
            var rules = new Rules(
                new Rule(() => true, typeof (SuccessfulResponseHandler), c => c.Set(ApplicationContextKeys.ContextName, "context-name"), (h, c, r) => null));
            rules.Evaluate(new ResponseHandlerProvider(new SuccessfulResponseHandler()), new ApplicationContext(), null);
        }

        private class SuccessfulResponseHandler : IResponseHandler
        {
            private bool wasCalled;

            public SuccessfulResponseHandler()
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

        private class UnsuccessfulResponseHandler : IResponseHandler
        {
            private bool wasCalled;

            public UnsuccessfulResponseHandler()
            {
                wasCalled = false;
            }

            public HandlerResult Handle(HttpResponseMessage response, ApplicationContext context)
            {
                wasCalled = true;
                return new HandlerResult(false, null);
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