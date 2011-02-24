using System;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.Keys;
using Restbucks.Client.RulesEngine;
using Restbucks.Client.States;
using Tests.Restbucks.Client.States.Helpers;

namespace Tests.Restbucks.Client.RulesEngine
{
    [TestFixture]
    public class WhenTests
    {
        [Test]
        public void ShouldReturnRuleThatUpdatesContextWithSuppliedAction()
        {
            IRule rule = When.IsTrue(() => true)
                .InvokeHandler(new DummyHandler())
                .UpdateContext(c => c.Set(new StringKey("key-name"), "value"))
                .ReturnState(CreateDummyState());

            var context = new ApplicationContext();

            rule.Evaluate(new HttpResponseMessage(), context);

            Assert.AreEqual("value", context.Get<string>(new StringKey("key-name")));
        }

        [Test]
        public void ShouldReturnRuleThatCreatesNewStateWithSuppliedFunction()
        {
            IRule rule = When.IsTrue(() => true)
                .InvokeHandler(new DummyHandler())
                .UpdateContext(DoNothingContextAction())
                .ReturnState(CreateDummyState());

            var result = rule.Evaluate(new HttpResponseMessage(), new ApplicationContext());

            Assert.IsInstanceOf(typeof (DummyState), result.Value);
        }

        [Test]
        public void ShouldReturnARuleContainingCreateResponseHandlerFunctionForSuppliedHandlerInstance()
        {
            var handler = new DummyHandler();

            var rule = When.IsTrue(() => true)
                .InvokeHandler(handler)
                .UpdateContext(DoNothingContextAction())
                .ReturnState(CreateDummyState());

            var func = rule.GetPrivateFieldValue<Func<IResponseHandler>>("createResponseHandler");

            Assert.AreEqual(handler, func());
        }

        [Test]
        public void ShouldReturnARuleContainingSuppliedCreateResponseHandlerFunction()
        {
            Func<IResponseHandler> f = () => new DummyHandler();

            var rule = When.IsTrue(() => true)
                .InvokeHandler(f)
                .UpdateContext(DoNothingContextAction())
                .ReturnState(CreateDummyState());

            var func = rule.GetPrivateFieldValue<Func<IResponseHandler>>("createResponseHandler");

            Assert.AreEqual(f, func);
        }

        private static Func<HttpResponseMessage, ApplicationContext, IState> CreateDummyState()
        {
            return (r, c) => new DummyState();
        }

        private static Action<ApplicationContext> DoNothingContextAction()
        {
            return c => { };
        }

        public class DummyHandler : IResponseHandler
        {
            public Result<HttpResponseMessage> Handle(HttpResponseMessage response, ApplicationContext context)
            {
                return new Result<HttpResponseMessage>(true, new HttpResponseMessage());
            }
        }

        public class DummyState : IState
        {
            public IState Apply(IResponseHandlers handlers)
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