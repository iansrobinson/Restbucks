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
        private static readonly ICondition DummyTrueCondition = CreateDummyCondition(true);
        private static readonly ICondition DummyFalseCondition = CreateDummyCondition(false);
        private static readonly IGenerateNextRequest DummyGenerateNextRequest = CreateDummyGenerateNextRequest();
        private static readonly CreateStateDelegate DummyCreateStateDelegate = (r, v, c) => NewState;
        private static readonly IClientCapabilities DummyClientCapabilities = MockRepository.GenerateStub<IClientCapabilities>();

        [Test]
        public void ShouldExecuteActionIfConditionIsApplicable()
        {
            var mockGenerateNextRequest = MockRepository.GenerateMock<IGenerateNextRequest>();
            mockGenerateNextRequest.Expect(a => a.Execute(PreviousResponse, StateVariables, DummyClientCapabilities)).Return(NewResponse);

            var rule = new Rule(DummyTrueCondition, mockGenerateNextRequest, DummyCreateStateDelegate);
            rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            mockGenerateNextRequest.VerifyAllExpectations();
        }

        [Test]
        public void ShouldCreateNewStateIfActionIsSuccessful()
        {
            var rule = new Rule(DummyTrueCondition, DummyGenerateNextRequest, DummyCreateStateDelegate);
            var result = rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            Assert.AreEqual(NewState, result.State);
        }

        [Test]
        public void ShouldReturnSuccessfulResultIfConditionIsApplicable()
        {
            var rule = new Rule(DummyTrueCondition, DummyGenerateNextRequest, DummyCreateStateDelegate);
            var result = rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void ShouldNotExecuteActionIfConditionIsNotApplicable()
        {
            var mockGenerateNextRequest = MockRepository.GenerateMock<IGenerateNextRequest>();
            mockGenerateNextRequest.AssertWasNotCalled(a => a.Execute(PreviousResponse, StateVariables, DummyClientCapabilities));

            var rule = new Rule(DummyFalseCondition, mockGenerateNextRequest, DummyCreateStateDelegate);
            rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            mockGenerateNextRequest.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnUnsuccessfulResultIfConditionIsNotApplicable()
        {
            var rule = new Rule(DummyFalseCondition, DummyGenerateNextRequest, DummyCreateStateDelegate);
            var result = rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            Assert.IsFalse(result.IsSuccessful);
            Assert.IsNull(result.State);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: condition")]
        public void ThrowsExceptionIfConditionIsNull()
        {
            new Rule(null, MockRepository.GenerateStub<IGenerateNextRequest>(), (r, v, c) => null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: actionInvoker")]
        public void ThrowsExceptionIfActionInvokerIsNull()
        {
            new Rule(MockRepository.GenerateStub<ICondition>(), null, (r, v, c) => null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: createState")]
        public void ThrowsExceptionIfCreateStateDelegateIsNull()
        {
            new Rule(MockRepository.GenerateStub<ICondition>(), MockRepository.GenerateStub<IGenerateNextRequest>(), null);
        }

        private static ICondition CreateDummyCondition(bool evaluatesTo)
        {
            var dummyCondition = MockRepository.GenerateStub<ICondition>();
            dummyCondition.Expect(c => c.IsApplicable(PreviousResponse, StateVariables)).Return(evaluatesTo);
            return dummyCondition;
        }

        private static IGenerateNextRequest CreateDummyGenerateNextRequest()
        {
            var dummyGenerateNextRequest = MockRepository.GenerateStub<IGenerateNextRequest>();
            dummyGenerateNextRequest.Expect(a => a.Execute(PreviousResponse, StateVariables, DummyClientCapabilities)).Return(NewResponse);
            return dummyGenerateNextRequest;
        }
    }
}