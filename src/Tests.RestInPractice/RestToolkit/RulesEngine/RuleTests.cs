using System;
using NUnit.Framework;
using RestInPractice.RestToolkit.RulesEngine;
using Rhino.Mocks;
using Tests.RestInPractice.RestToolkit.RulesEngine.Util;

namespace Tests.RestInPractice.RestToolkit.RulesEngine
{
    [TestFixture]
    public class RuleTests
    {
        [Test]
        public void ShouldExecuteActionIfConditionIsApplicable()
        {
            var mockGenerateNextRequest = MockRepository.GenerateMock<IRequestAction>();
            mockGenerateNextRequest.Expect(a => a.Execute(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities)).Return(Dummy.NewResponse);

            var rule = new Rule(Dummy.Condition(true), mockGenerateNextRequest, Dummy.CreateNextState());
            rule.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

            mockGenerateNextRequest.VerifyAllExpectations();
        }

        [Test]
        public void ShouldCreateNewStateIfActionIsSuccessful()
        {
            var rule = new Rule(Dummy.Condition(true), Dummy.GenerateNextRequest(), Dummy.CreateNextState());
            var result = rule.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

            Assert.AreEqual(Dummy.NewState, result.State);
        }

        [Test]
        public void ShouldReturnSuccessfulResultIfConditionIsApplicable()
        {
            var rule = new Rule(Dummy.Condition(true), Dummy.GenerateNextRequest(), Dummy.CreateNextState());
            var result = rule.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void ShouldNotExecuteActionIfConditionIsNotApplicable()
        {
            var mockGenerateNextRequest = MockRepository.GenerateMock<IRequestAction>();
            mockGenerateNextRequest.AssertWasNotCalled(a => a.Execute(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities));

            var rule = new Rule(Dummy.Condition(false), mockGenerateNextRequest, Dummy.CreateNextState());
            rule.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

            mockGenerateNextRequest.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnUnsuccessfulResultIfConditionIsNotApplicable()
        {
            var rule = new Rule(Dummy.Condition(false), Dummy.GenerateNextRequest(), Dummy.CreateNextState());
            var result = rule.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

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
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: createNextState")]
        public void ThrowsExceptionIfCreateStateDelegateIsNull()
        {
            new Rule(MockRepository.GenerateStub<ICondition>(), MockRepository.GenerateStub<IRequestAction>(), null);
        }
    }
}