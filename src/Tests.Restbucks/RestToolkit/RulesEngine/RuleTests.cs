using System;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.RestToolkit.RulesEngine;
using Rhino.Mocks;

namespace Tests.Restbucks.RestToolkit.RulesEngine
{
    [TestFixture]
    public class RuleTests
    {
        private static readonly HttpResponseMessage PreviousResponse = new HttpResponseMessage();
        private static readonly ApplicationStateVariables StateVariables = new ApplicationStateVariables();
        private static readonly HttpResponseMessage NewResponse = new HttpResponseMessage();
        private static readonly IState NewState = MockRepository.GenerateStub<IState>();
        private static readonly IClientCapabilities DummyClientCapabilities = MockRepository.GenerateStub<IClientCapabilities>();

        [Test]
        public void ShouldExecuteActionIfConditionIsApplicable()
        {
            var mockGenerateNextRequest = MockRepository.GenerateMock<IRequestAction>();
            mockGenerateNextRequest.Expect(a => a.Execute(PreviousResponse, StateVariables, DummyClientCapabilities)).Return(NewResponse);

            var rule = new Rule(CreateDummyCondition(true), mockGenerateNextRequest, CreateDummyCreateNextState());
            rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            mockGenerateNextRequest.VerifyAllExpectations();
        }

        [Test]
        public void ShouldCreateNewStateIfActionIsSuccessful()
        {
            var rule = new Rule(CreateDummyCondition(true), CreateDummyGenerateNextRequest(), CreateDummyCreateNextState());
            var result = rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            Assert.AreEqual(NewState, result.State);
        }

        [Test]
        public void ShouldReturnSuccessfulResultIfConditionIsApplicable()
        {
            var rule = new Rule(CreateDummyCondition(true), CreateDummyGenerateNextRequest(), CreateDummyCreateNextState());
            var result = rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void ShouldNotExecuteActionIfConditionIsNotApplicable()
        {
            var mockGenerateNextRequest = MockRepository.GenerateMock<IRequestAction>();
            mockGenerateNextRequest.AssertWasNotCalled(a => a.Execute(PreviousResponse, StateVariables, DummyClientCapabilities));

            var rule = new Rule(CreateDummyCondition(false), mockGenerateNextRequest, CreateDummyCreateNextState());
            rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            mockGenerateNextRequest.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnUnsuccessfulResultIfConditionIsNotApplicable()
        {
            var rule = new Rule(CreateDummyCondition(false), CreateDummyGenerateNextRequest(), CreateDummyCreateNextState());
            var result = rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            Assert.IsFalse(result.IsSuccessful);
            Assert.IsNull(result.State);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: condition")]
        public void ThrowsExceptionIfConditionIsNull()
        {
            new Rule(null, MockRepository.GenerateStub<IRequestAction>(), MockRepository.GenerateStub<ICreateNextState>());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: requestAction")]
        public void ThrowsExceptionIfActionInvokerIsNull()
        {
            new Rule(MockRepository.GenerateStub<ICondition>(), null, MockRepository.GenerateStub<ICreateNextState>());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: createNextState")]
        public void ThrowsExceptionIfCreateStateDelegateIsNull()
        {
            new Rule(MockRepository.GenerateStub<ICondition>(), MockRepository.GenerateStub<IRequestAction>(), null);
        }

        private static ICondition CreateDummyCondition(bool evaluatesTo)
        {
            var dummyCondition = MockRepository.GenerateStub<ICondition>();
            dummyCondition.Expect(c => c.IsApplicable(PreviousResponse, StateVariables)).Return(evaluatesTo);
            return dummyCondition;
        }

        private static IRequestAction CreateDummyGenerateNextRequest()
        {
            var dummyGenerateNextRequest = MockRepository.GenerateStub<IRequestAction>();
            dummyGenerateNextRequest.Expect(g => g.Execute(PreviousResponse, StateVariables, DummyClientCapabilities)).Return(NewResponse);
            return dummyGenerateNextRequest;
        }

        private static ICreateNextState CreateDummyCreateNextState()
        {
            var dummyCreateNextState = MockRepository.GenerateStub<ICreateNextState>();
            dummyCreateNextState.Expect(c => c.Execute(NewResponse, StateVariables, DummyClientCapabilities)).Return(NewState);
            return dummyCreateNextState;
        }
    }
}