using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using NUnit.Framework;
using Restbucks.NewClient;
using Rhino.Mocks;

namespace Tests.Restbucks.NewClient
{
    [TestFixture]
    public class RuleTests
    {
        [Test]
        public void ShouldExecuteActionIfConditionIsApplicable()
        {
            var condition = MockRepository.GenerateStub<ICondition>();
            condition.Expect(c => c.IsApplicable(new HttpResponseMessage())).IgnoreArguments().Return(true);

            var action = MockRepository.GenerateStrictMock<IAction>();
            action.Expect(a => a.Execute()).Return(new HttpResponseMessage());

            var rule = new Rule(condition, action);
            rule.Evaluate(new HttpResponseMessage());

            action.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnSuccessfulResultIfActionIsExecuted()
        {
            var response = new HttpResponseMessage();
            
            var condition = MockRepository.GenerateStub<ICondition>();
            condition.Expect(c => c.IsApplicable(new HttpResponseMessage())).IgnoreArguments().Return(true);

            var action = MockRepository.GenerateStub<IAction>();
            action.Expect(a => a.Execute()).Return(response);

            var rule = new Rule(condition, action);
            var result = rule.Evaluate(new HttpResponseMessage());

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(response, result.Response);
        }

        [Test]
        public void ShouldNotExecuteActionIfConditionIsNotApplicable()
        {
            var condition = MockRepository.GenerateStub<ICondition>();
            condition.Expect(c => c.IsApplicable(new HttpResponseMessage())).IgnoreArguments().Return(false);

            var action = MockRepository.GenerateStrictMock<IAction>();
            action.AssertWasNotCalled(a => a.Execute());

            var rule = new Rule(condition, action);
            rule.Evaluate(new HttpResponseMessage());

            action.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnUnsuccessfulResultIfActionIsNotExecuted()
        {
            var condition = MockRepository.GenerateStub<ICondition>();
            condition.Expect(c => c.IsApplicable(new HttpResponseMessage())).IgnoreArguments().Return(false);

            var action = MockRepository.GenerateStub<IAction>();

            var rule = new Rule(condition, action);
            var result = rule.Evaluate(new HttpResponseMessage());

            Assert.IsFalse(result.IsSuccessful);
            Assert.IsNull(result.Response);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: condition")]
        public void ThrowsExceptionIfConditionIsNull()
        {
            new Rule(null, MockRepository.GenerateStub<IAction>());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: action")]
        public void ThrowsExceptionIfActionIsNull()
        {
            new Rule(MockRepository.GenerateStub<ICondition>(), null);
        }
    }
}
