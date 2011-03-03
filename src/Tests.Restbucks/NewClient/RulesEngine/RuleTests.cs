using System;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.NewClient.RulesEngine;
using Rhino.Mocks;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class RuleTests
    {
        private static readonly HttpResponseMessage PreviousResponse = new HttpResponseMessage();
        private static readonly HttpResponseMessage NewResponse = new HttpResponseMessage();
        private static readonly IState NewState = new DummyState();
        
        [Test]
        public void ShouldExecuteActionIfConditionIsApplicable()
        {
            var condition = MockRepository.GenerateStub<ICondition>();
            condition.Expect(c => c.IsApplicable(PreviousResponse)).Return(true);

            var action = MockRepository.GenerateMock<IAction>();
            action.Expect(a => a.Execute()).Return(NewResponse);

            var stateFactory = MockRepository.GenerateStub<IStateFactory>();

            var rule = new Rule(condition, action, stateFactory);
            rule.Evaluate(PreviousResponse);

            action.VerifyAllExpectations();
        }

        [Test]
        public void ShouldCreateNewStateIfActionIsSuccessful()
        {
            var condition = MockRepository.GenerateStub<ICondition>();
            condition.Expect(c => c.IsApplicable(PreviousResponse)).Return(true);

            var action = MockRepository.GenerateStub<IAction>();
            action.Expect(a => a.Execute()).Return(NewResponse);

            var stateFactory = MockRepository.GenerateStub<IStateFactory>();
            stateFactory.Expect(f => f.Create(NewResponse)).IgnoreArguments().Return(NewState);

            var rule = new Rule(condition, action, stateFactory);
            var result = rule.Evaluate(PreviousResponse);

            Assert.AreEqual(NewState, result.State);
        }

        [Test]
        public void ShouldReturnSuccessfulResultIfConditionIsApplicable()
        {
            var condition = MockRepository.GenerateStub<ICondition>();
            condition.Expect(c => c.IsApplicable(PreviousResponse)).Return(true);

            var action = MockRepository.GenerateStub<IAction>();
            action.Expect(a => a.Execute()).Return(NewResponse);

            var stateFactory = MockRepository.GenerateStub<IStateFactory>();           
            stateFactory.Expect(f => f.Create(NewResponse)).Return(NewState);

            var rule = new Rule(condition, action, stateFactory);
            var result = rule.Evaluate(PreviousResponse);

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void ShouldNotExecuteActionIfConditionIsNotApplicable()
        {
            var condition = MockRepository.GenerateStub<ICondition>();
            condition.Expect(c => c.IsApplicable(PreviousResponse)).Return(false);

            var action = MockRepository.GenerateMock<IAction>();
            action.AssertWasNotCalled(a => a.Execute());

            var stateFactory = MockRepository.GenerateStub<IStateFactory>();

            var rule = new Rule(condition, action, stateFactory);
            rule.Evaluate(PreviousResponse);

            action.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnUnsuccessfulResultIfConditionIsNotApplicable()
        {
            var condition = MockRepository.GenerateStub<ICondition>();
            condition.Expect(c => c.IsApplicable(PreviousResponse)).Return(false);

            var action = MockRepository.GenerateStub<IAction>();
            var stateFactory = MockRepository.GenerateStub<IStateFactory>();

            var rule = new Rule(condition, action, stateFactory);
            var result = rule.Evaluate(PreviousResponse);

            Assert.IsFalse(result.IsSuccessful);
            Assert.IsNull(result.State);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: condition")]
        public void ThrowsExceptionIfConditionIsNull()
        {
            new Rule(null, MockRepository.GenerateStub<IAction>(), MockRepository.GenerateStub<IStateFactory>());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: action")]
        public void ThrowsExceptionIfActionIsNull()
        {
            new Rule(MockRepository.GenerateStub<ICondition>(), null, MockRepository.GenerateStub<IStateFactory>());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: stateFactory")]
        public void ThrowsExceptionIfStateFactoryIsNull()
        {
            new Rule(MockRepository.GenerateStub<ICondition>(), MockRepository.GenerateStub<IAction>(), null);
        }

        private class DummyState : IState
        {          
        }
    }
}