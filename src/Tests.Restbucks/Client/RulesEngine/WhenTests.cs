using System;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.Http;
using Restbucks.Client.Keys;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.RulesEngine;

namespace Tests.Restbucks.Client.RulesEngine
{
    [TestFixture]
    public class WhenTests
    {
        [Test]
        public void ShouldReturnRuleThatUpdatesContextWithSuppliedAction()
        {
            IRule rule = When.IsTrue(() => true)
                .InvokeHandler<DummyHandler>()
                .UpdateContext(c => c.Set(new StringKey("key-name"), "value"))
                .ReturnState(CreateDummyState());

            var context = new ApplicationContext();

            rule.CreateNewState(new HttpResponseMessage(), context, HttpClientProvider.Instance);

            Assert.AreEqual("value", context.Get<string>(new StringKey("key-name")));
        }

        [Test]
        public void ShouldReturnRuleThatCreatesNewStateWithSuppliedFunction()
        {
            IRule rule = When.IsTrue(() => true)
                .InvokeHandler<DummyHandler>()
                .UpdateContext(DoNothingContextAction())
                .ReturnState(CreateDummyState());

            var state = rule.CreateNewState(new HttpResponseMessage(), new ApplicationContext(), HttpClientProvider.Instance);

            Assert.IsInstanceOf(typeof(DummyState), state);
        }

        private static Func<HttpResponseMessage, ApplicationContext, IHttpClientProvider, IState> CreateDummyState()
        {
            return (r, c, p) => new DummyState();
        }

        private static Action<ApplicationContext> DoNothingContextAction()
        {
            return c => { };
        }

        public class DummyHandler : IResponseHandler
        {
            public Result<HttpResponseMessage> Handle(HttpResponseMessage response, ApplicationContext context, IHttpClientProvider clientProvider)
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