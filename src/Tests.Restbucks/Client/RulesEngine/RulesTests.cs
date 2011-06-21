//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using NUnit.Framework;
//using Restbucks.Client;
//using Restbucks.Client.Keys;
//using Restbucks.Client.RulesEngine;
//using Restbucks.Client.States;
//
//namespace Tests.Restbucks.Client.RulesEngine
//{
//    [TestFixture]
//    public class RulesTests
//    {
//        [Test]
//        public void ShouldProcessEachRuleInTheOrderItWasAddedToRules()
//        {
//            var processOrder = new Queue<string>();
//
//            var rules = new Rules(
//                new Rule(() =>
//                             {
//                                 processOrder.Enqueue("first");
//                                 return false;
//                             },
//                         () => new SuccessfulResponseHandler(),
//                         c => c.Set(ApplicationContextKeys.SemanticContext, "context-name"),
//                         (r, c) => new DummyState()),
//                new Rule(() =>
//                             {
//                                 processOrder.Enqueue("second");
//                                 return false;
//                             }, () => new SuccessfulResponseHandler(),
//                         c => c.Set(ApplicationContextKeys.SemanticContext, "context-name"),
//                         (r, c) => new DummyState()),
//                new Rule(() =>
//                             {
//                                 processOrder.Enqueue("third");
//                                 return false;
//                             }, () => new SuccessfulResponseHandler(),
//                         c => c.Set(ApplicationContextKeys.SemanticContext, "context-name"),
//                         (r, c) => new DummyState())
//                );
//
//            rules.Evaluate(null, new ApplicationContext());
//
//            Assert.IsTrue(processOrder.SequenceEqual(new[] {"first", "second", "third"}));
//        }
//
//        [Test]
//        public void ShouldOnlyEvaluateRuleIfItIsApplicable()
//        {
//            var handler = new SuccessfulResponseHandler();
//            var rules = new Rules(
//                new Rule(() => true,
//                         () => handler,
//                         c => c.Set(ApplicationContextKeys.SemanticContext, "context-name"),
//                         (r, c) => new DummyState()));
//
//            Assert.IsFalse(handler.WasCalled);
//
//            rules.Evaluate(null, new ApplicationContext());
//
//            Assert.IsTrue(handler.WasCalled);
//        }
//
//        [Test]
//        public void ShouldMoveToNextRuleIfEvaluatingARuleDoesNotSucceed()
//        {
//            var firstHandler = new UnsuccessfulResponseHandler();
//            var secondHandler = new SuccessfulResponseHandler();
//
//            var rules = new Rules(
//                new Rule(() => true,
//                         () => firstHandler,
//                         c => c.Set(ApplicationContextKeys.SemanticContext, "context-name"),
//                         (r, c) => new DummyState()),
//                new Rule(() => true,
//                         () => secondHandler,
//                         c => c.Set(ApplicationContextKeys.SemanticContext, "context-name"),
//                         (r, c) => new DummyState()));
//
//            rules.Evaluate(null, new ApplicationContext());
//
//            Assert.IsTrue(firstHandler.WasCalled);
//            Assert.IsTrue(secondHandler.WasCalled);
//        }
//
//        [Test]
//        [ExpectedException(ExpectedException = typeof (NullStateException))]
//        public void ShouldThrowExceptionIfCreateStateFunctionReturnsNull()
//        {
//            var rules = new Rules(
//                new Rule(() => true,
//                         () => new SuccessfulResponseHandler(),
//                         c => c.Set(ApplicationContextKeys.SemanticContext, "context-name"),
//                         (r, c) => null));
//            rules.Evaluate(null, new ApplicationContext());
//        }
//
//        [Test]
//        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "ElseRule must be last rule in list.\r\nParameter name: rules")]
//        public void ShouldThrowExceptionIfElseRuleIsNotLastRule()
//        {
//            new Rules(
//                new Rule(() => true,
//                         () => new UnsuccessfulResponseHandler(),
//                         c => c.Set(ApplicationContextKeys.SemanticContext, "context-name"),
//                         (r, c) => new DummyState()),
//                new ElseRule(c => c.Set(ApplicationContextKeys.SemanticContext, "context-name"), (h, c) => new DummyState()),
//                new Rule(() => true,
//                         () => new SuccessfulResponseHandler(),
//                         c => c.Set(ApplicationContextKeys.SemanticContext, "context-name"),
//                         (r, c) => new DummyState()));
//        }
//
//        [Test]
//        public void ShouldReturnTerminalStateWhenNoRuleSucceedsAndNoElseRuleIsSupplied()
//        {
//            var rules = new Rules();
//            var state = rules.Evaluate(new HttpResponseMessage(), new ApplicationContext());
//
//            Assert.IsInstanceOf(typeof (TerminalState), state);
//        }
//
//        [Test]
//        public void ShouldEvaluateSuppliedElseRuleIfNoOtherRuleSucceeds()
//        {
//            var rules = new Rules(
//                new Rule(() => false, () => new UnsuccessfulResponseHandler(), c => { }, (r, c) => new DummyState()),
//                new ElseRule(c => c.Set(new StringKey("key"), "value"), (r, c) => new DummyTerminalState()));
//
//            var context = new ApplicationContext();
//
//            var state = rules.Evaluate(new HttpResponseMessage(), context);
//
//            Assert.AreEqual("value", context.Get<string>(new StringKey("key")));
//            Assert.IsInstanceOf(typeof (DummyTerminalState), state);
//        }
//
//        private class SuccessfulResponseHandler : IResponseHandler
//        {
//            private bool wasCalled;
//
//            public SuccessfulResponseHandler()
//            {
//                wasCalled = false;
//            }
//
//            public Result<HttpResponseMessage> Handle(HttpResponseMessage response, ApplicationContext context)
//            {
//                wasCalled = true;
//                return new Result<HttpResponseMessage>(true, new HttpResponseMessage());
//            }
//
//            public bool WasCalled
//            {
//                get { return wasCalled; }
//            }
//        }
//
//        private class UnsuccessfulResponseHandler : IResponseHandler
//        {
//            private bool wasCalled;
//
//            public UnsuccessfulResponseHandler()
//            {
//                wasCalled = false;
//            }
//
//            public Result<HttpResponseMessage> Handle(HttpResponseMessage response, ApplicationContext context)
//            {
//                wasCalled = true;
//                return new Result<HttpResponseMessage>(false, null);
//            }
//
//            public bool WasCalled
//            {
//                get { return wasCalled; }
//            }
//        }
//
//        private class DummyState : IState
//        {
//            public IState NextState(IResponseHandlers handlers)
//            {
//                throw new NotImplementedException();
//            }
//
//            public bool IsTerminalState
//            {
//                get { throw new NotImplementedException(); }
//            }
//        }
//
//        private class DummyTerminalState : IState
//        {
//            public IState NextState(IResponseHandlers handlers)
//            {
//                throw new NotImplementedException();
//            }
//
//            public bool IsTerminalState
//            {
//                get { throw new NotImplementedException(); }
//            }
//        }
//    }
//}