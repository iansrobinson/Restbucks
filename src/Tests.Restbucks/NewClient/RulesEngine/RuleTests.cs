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
        private static readonly ApplicationContext Context = new ApplicationContext();
        private static readonly HttpResponseMessage NewResponse = new HttpResponseMessage();
        private static readonly IState NewState = new DummyState();
        
        [Test]
        public void ShouldExecuteActionIfConditionIsApplicable()
        {
            var dummyCondition = MockRepository.GenerateStub<ICondition>();
            dummyCondition.Expect(c => c.IsApplicable(PreviousResponse, Context)).Return(true);

            var mockAction = MockRepository.GenerateMock<IActionInvoker>();
            mockAction.Expect(a => a.Invoke(PreviousResponse, Context)).Return(NewResponse);

            var dummyStateFactory = MockRepository.GenerateStub<IStateFactory>();

            var rule = new Rule(dummyCondition, mockAction, dummyStateFactory);
            rule.Evaluate(PreviousResponse, Context);

            mockAction.VerifyAllExpectations();
        }

        [Test]
        public void ShouldCreateNewStateIfActionIsSuccessful()
        {
            var dummyCondition = MockRepository.GenerateStub<ICondition>();
            dummyCondition.Expect(c => c.IsApplicable(PreviousResponse, Context)).Return(true);

            var dummyAction = MockRepository.GenerateStub<IActionInvoker>();
            dummyAction.Expect(a => a.Invoke(PreviousResponse, Context)).Return(NewResponse);

            var dummyStateFactory = MockRepository.GenerateStub<IStateFactory>();
            dummyStateFactory.Expect(f => f.Create(NewResponse, Context)).IgnoreArguments().Return(NewState);

            var rule = new Rule(dummyCondition, dummyAction, dummyStateFactory);
            var result = rule.Evaluate(PreviousResponse, Context);

            Assert.AreEqual(NewState, result.State);
        }

        [Test]
        public void ShouldReturnSuccessfulResultIfConditionIsApplicable()
        {
            var dummyCondition = MockRepository.GenerateStub<ICondition>();
            dummyCondition.Expect(c => c.IsApplicable(PreviousResponse, Context)).Return(true);

            var dummyAction = MockRepository.GenerateStub<IActionInvoker>();
            dummyAction.Expect(a => a.Invoke(PreviousResponse, Context)).Return(NewResponse);

            var dummyStateFactory = MockRepository.GenerateStub<IStateFactory>();           
            dummyStateFactory.Expect(f => f.Create(NewResponse, Context)).Return(NewState);

            var rule = new Rule(dummyCondition, dummyAction, dummyStateFactory);
            var result = rule.Evaluate(PreviousResponse, Context);

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void ShouldNotExecuteActionIfConditionIsNotApplicable()
        {
            var dummyCondition = MockRepository.GenerateStub<ICondition>();
            dummyCondition.Expect(c => c.IsApplicable(PreviousResponse, Context)).Return(false);

            var mockAction = MockRepository.GenerateMock<IActionInvoker>();
            mockAction.AssertWasNotCalled(a => a.Invoke(PreviousResponse, Context));

            var dummyStateFactory = MockRepository.GenerateStub<IStateFactory>();

            var rule = new Rule(dummyCondition, mockAction, dummyStateFactory);
            rule.Evaluate(PreviousResponse, Context);

            mockAction.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnUnsuccessfulResultIfConditionIsNotApplicable()
        {
            var dummyCondition = MockRepository.GenerateStub<ICondition>();
            dummyCondition.Expect(c => c.IsApplicable(PreviousResponse, Context)).Return(false);

            var dummyAction = MockRepository.GenerateStub<IActionInvoker>();
            var dummyStateFactory = MockRepository.GenerateStub<IStateFactory>();

            var rule = new Rule(dummyCondition, dummyAction, dummyStateFactory);
            var result = rule.Evaluate(PreviousResponse, Context);

            Assert.IsFalse(result.IsSuccessful);
            Assert.IsNull(result.State);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: condition")]
        public void ThrowsExceptionIfConditionIsNull()
        {
            new Rule(null, MockRepository.GenerateStub<IActionInvoker>(), MockRepository.GenerateStub<IStateFactory>());
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
            new Rule(MockRepository.GenerateStub<ICondition>(), MockRepository.GenerateStub<IActionInvoker>(), null);
        }

        private class DummyState : IState
        {          
        }
    }
}