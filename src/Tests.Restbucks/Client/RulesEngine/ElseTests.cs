using System;
using System.Collections.Generic;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.Keys;
using Restbucks.Client.ResponseHandlers;
using Restbucks.Client.RulesEngine;
using Restbucks.Client.States;
using Tests.Restbucks.Client.States.Helpers;

namespace Tests.Restbucks.Client.RulesEngine
{
    [TestFixture]
    public class ElseTests
    {
        [Test]
        public void ShouldReturnRuleWhichIsAlwaysApplicable()
        {
            IRule rule = Else.UpdateContext(c => { }).ReturnState((h, c, r) => new DummyState());
            Assert.IsTrue(rule.IsApplicable);
        }

        [Test]
        public void ShouldReturnRuleThatUpdatesContextWithSuppliedAction()
        {
            IRule rule = Else.UpdateContext(c => c.Set(new StringKey("key-name"), "value")).ReturnState((h, c, r) => new DummyState());
            var context = new ApplicationContext();

            rule.CreateNewState(new ResponseHandlerProvider(), context, new HttpResponseMessage());

            Assert.AreEqual("value", context.Get<string>(new StringKey("key-name")));
        }

        [Test]
        public void ShouldReturnRuleThatCreatesNewStateWithSuppliedFunction()
        {
            IRule rule = Else.UpdateContext(c => c.Set(new StringKey("key-name"), "value")).ReturnState((h, c, r) => new DummyState());
            var state = rule.CreateNewState(new ResponseHandlerProvider(), new ApplicationContext(), new HttpResponseMessage());

            Assert.IsInstanceOf(typeof (DummyState), state);
        }

        [Test]
        public void IfNoUpdateContextActionIsSuppliedShouldNotModifyContext()
        {
            IRule rule = Else.ReturnState((h, c, r) => new DummyState());
            var context = new ApplicationContext();

            rule.CreateNewState(new ResponseHandlerProvider(), context, new HttpResponseMessage());

            var dict = PrivateField.GetValue<Dictionary<IKey, object>>("values", context);
            Assert.AreEqual(0, dict.Keys.Count);
        }

        [Test]
        public void IfTerminateIsSpecifiedInPlaceOfCreateStateFunctionShouldReturnRuleThatCreatesTerminalState()
        {
            IRule rule = Else.UpdateContext(c => { }).Terminate();
            var state = rule.CreateNewState(new ResponseHandlerProvider(), new ApplicationContext(), new HttpResponseMessage());
            
            Assert.IsInstanceOf(typeof (TerminalState), state);
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