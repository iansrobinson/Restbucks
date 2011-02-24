using System;
using System.Collections.Generic;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.Http;
using Restbucks.Client.Keys;
using Restbucks.Client.RulesEngine;
using Restbucks.Client.States;
using Tests.Restbucks.Client.States.Helpers;

namespace Tests.Restbucks.Client.RulesEngine
{
    [TestFixture]
    public class ElseTests
    {
        [Test]
        public void EvaluateShouldReturnSuccessfulResult()
        {
            IRule rule = Else.UpdateContext(c => { }).ReturnState((h, c, r) => new DummyState());
            var result = rule.Evaluate(new HttpResponseMessage(), new ApplicationContext(), HttpClientProvider.Instance);
            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void EvaluateShouldReturnSuppliedState()
        {
            IRule rule = Else.UpdateContext(c => { }).ReturnState((h, c, r) => new DummyState());
            var result = rule.Evaluate(new HttpResponseMessage(), new ApplicationContext(), HttpClientProvider.Instance);
            Assert.IsInstanceOf(typeof(DummyState), result.Value);
        }

        [Test]
        public void ShouldReturnRuleThatUpdatesContextWithSuppliedAction()
        {
            IRule rule = Else.UpdateContext(c => c.Set(new StringKey("key-name"), "value")).ReturnState((h, c, r) => new DummyState());
            var context = new ApplicationContext();

            rule.Evaluate(new HttpResponseMessage(), context, HttpClientProvider.Instance);

            Assert.AreEqual("value", context.Get<string>(new StringKey("key-name")));
        }

        [Test]
        public void ShouldReturnRuleThatCreatesNewStateWithSuppliedFunction()
        {
            IRule rule = Else.UpdateContext(c => c.Set(new StringKey("key-name"), "value")).ReturnState((h, c, r) => new DummyState());
            var result = rule.Evaluate(new HttpResponseMessage(), new ApplicationContext(), HttpClientProvider.Instance);

            Assert.IsInstanceOf(typeof (DummyState), result.Value);
        }

        [Test]
        public void IfNoUpdateContextActionIsSuppliedShouldNotModifyContext()
        {
            IRule rule = Else.ReturnState((h, c, r) => new DummyState());
            var context = new ApplicationContext();

            rule.Evaluate(new HttpResponseMessage(), context, HttpClientProvider.Instance);

            var dict = context.GetPrivateFieldValue<Dictionary<IKey, object>>("values");
            Assert.AreEqual(0, dict.Keys.Count);
        }

        [Test]
        public void IfTerminateIsSpecifiedInPlaceOfCreateStateFunctionShouldReturnRuleThatCreatesTerminalState()
        {
            IRule rule = Else.UpdateContext(c => { }).Terminate();
            var result = rule.Evaluate(new HttpResponseMessage(), new ApplicationContext(), HttpClientProvider.Instance);

            Assert.IsInstanceOf(typeof (TerminalState), result.Value);
        }

        private class DummyState : IState
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