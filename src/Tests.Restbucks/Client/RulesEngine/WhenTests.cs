using System;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.Keys;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.RulesEngine;

namespace Tests.Restbucks.Client.RulesEngine
{
    [TestFixture]
    public class WhenTests
    {
        [Test]
        public void ShouldReturnRuleThatIsApplicableAccordingToSuppliedCondition()
        {
            IRule rule1 = When.IsTrue(() => true)
                .InvokeHandler<DummyHandler>()
                .UpdateContext(DoNothingContextAction())
                .ReturnState(CreateDummyState());
            Assert.IsTrue(rule1.IsApplicable);

            IRule rule2 = When.IsTrue(() => false)
                .InvokeHandler<DummyHandler>()
                .UpdateContext(DoNothingContextAction())
                .ReturnState(CreateDummyState());
            Assert.IsFalse(rule2.IsApplicable);
        }

        [Test]
        public void ShouldReturnRuleWithSuppliedResponseHandlerType()
        {
            IRule rule = When.IsTrue(() => true)
                .InvokeHandler<DummyHandler>()
                .UpdateContext(DoNothingContextAction())
                .ReturnState(CreateDummyState());

            Assert.AreEqual(typeof(DummyHandler), rule.ResponseHandlerType);
        }

        [Test]
        public void ShouldReturnRuleThatUpdatesContextWithSuppliedAction()
        {
            IRule rule = When.IsTrue(() => true)
                .InvokeHandler<DummyHandler>()
                .UpdateContext(c => c.Set(new StringKey("key-name"), "value"))
                .ReturnState(CreateDummyState());

            var context = new ApplicationContext();

            rule.ContextAction(context);

            Assert.AreEqual("value", context.Get<string>(new StringKey("key-name")));
        }

        [Test]
        public void ShouldReturnRuleThatCreatesNewStateWithSuppliedFunction()
        {
            IRule rule = When.IsTrue(() => true)
                .InvokeHandler<DummyHandler>()
                .UpdateContext(DoNothingContextAction())
                .ReturnState(CreateDummyState());

            var state = rule.CreateState(new ResponseHandlerProvider(), new ApplicationContext(), new HttpResponseMessage());

            Assert.IsInstanceOf(typeof(DummyState), state);
        }

        private static Func<IResponseHandlerProvider, ApplicationContext, HttpResponseMessage, IState> CreateDummyState()
        {
            return (h, c, m) => new DummyState();
        }

        private static Action<ApplicationContext> DoNothingContextAction()
        {
            return c => { };
        }

        public class DummyHandler : IResponseHandler
        {
            public HandlerResult Handle(HttpResponseMessage response, ApplicationContext context)
            {
                throw new NotImplementedException();
            }
        }

        public class DummyState : IState
        {
            public IState Apply()
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