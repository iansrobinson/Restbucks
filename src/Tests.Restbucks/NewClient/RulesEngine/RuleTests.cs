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
        private static readonly ApplicationStateVariables StateVariables = new ApplicationStateVariables();
        private static readonly HttpResponseMessage NewResponse = new HttpResponseMessage();
        private static readonly IState NewState = MockRepository.GenerateStub<IState>();
        private static readonly ICondition DummyTrueCondition = CreateDummyCondition(true);
        private static readonly ICondition DummyFalseCondition = CreateDummyCondition(false);
        private static readonly IActionInvoker DummyActionInvoker = CreateDummyActionInvoker();
        private static readonly IStateFactory DummyStateFactory = CreateDummyStateFactory();
        private static readonly IClientCapabilities DummyClientCapabilities = MockRepository.GenerateStub<IClientCapabilities>();

        [Test]
        public void ShouldExecuteActionIfConditionIsApplicable()
        {
            var mockAction = MockRepository.GenerateMock<IActionInvoker>();
            mockAction.Expect(a => a.Invoke(PreviousResponse, StateVariables, DummyClientCapabilities)).Return(NewResponse);

            var rule = new Rule(DummyTrueCondition, mockAction, DummyStateFactory.Create);
            rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            mockAction.VerifyAllExpectations();
        }

        [Test]
        public void ShouldCreateNewStateIfActionIsSuccessful()
        {
            var rule = new Rule(DummyTrueCondition, DummyActionInvoker, DummyStateFactory.Create);
            var result = rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            Assert.AreEqual(NewState, result.State);
        }

        [Test]
        public void ShouldReturnSuccessfulResultIfConditionIsApplicable()
        {
            var rule = new Rule(DummyTrueCondition, DummyActionInvoker, DummyStateFactory.Create);
            var result = rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void ShouldNotExecuteActionIfConditionIsNotApplicable()
        {
            var mockAction = MockRepository.GenerateMock<IActionInvoker>();
            mockAction.AssertWasNotCalled(a => a.Invoke(PreviousResponse, StateVariables, DummyClientCapabilities));

            var rule = new Rule(DummyFalseCondition, mockAction, DummyStateFactory.Create);
            rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            mockAction.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnUnsuccessfulResultIfConditionIsNotApplicable()
        {
            var rule = new Rule(DummyFalseCondition, DummyActionInvoker, DummyStateFactory.Create);
            var result = rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            Assert.IsFalse(result.IsSuccessful);
            Assert.IsNull(result.State);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: condition")]
        public void ThrowsExceptionIfConditionIsNull()
        {
            new Rule(null, MockRepository.GenerateStub<IActionInvoker>(), MockRepository.GenerateStub<IStateFactory>().Create);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: actionInvoker")]
        public void ThrowsExceptionIfActionInvokerIsNull()
        {
            new Rule(MockRepository.GenerateStub<ICondition>(), null, MockRepository.GenerateStub<IStateFactory>().Create);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: createState")]
        public void ThrowsExceptionIfCreateStateDelegateIsNull()
        {
            new Rule(MockRepository.GenerateStub<ICondition>(), MockRepository.GenerateStub<IActionInvoker>(), null);
        }

        private static ICondition CreateDummyCondition(bool evaluatesTo)
        {
            var dummyCondition = MockRepository.GenerateStub<ICondition>();
            dummyCondition.Expect(c => c.IsApplicable(PreviousResponse, StateVariables)).Return(evaluatesTo);
            return dummyCondition;
        }

        private static IActionInvoker CreateDummyActionInvoker()
        {
            var dummyAction = MockRepository.GenerateStub<IActionInvoker>();
            dummyAction.Expect(a => a.Invoke(PreviousResponse, StateVariables, DummyClientCapabilities)).Return(NewResponse);
            return dummyAction;
        }

        private static IStateFactory CreateDummyStateFactory()
        {
            var dummyStateFactory = MockRepository.GenerateStub<IStateFactory>();
            dummyStateFactory.Expect(f => f.Create(NewResponse, StateVariables, DummyClientCapabilities)).IgnoreArguments().Return(NewState);
            return dummyStateFactory;
        }
    }
}