using System.Net.Http;
using NUnit.Framework;
using Restbucks.NewClient.RulesEngine;
using Rhino.Mocks;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class RulesTests
    {
        [Test]
        public void ShouldCallEachRuleInOrderItWasAddedToRules()
        {
            var response = new HttpResponseMessage();

            var mocks = new MockRepository();

            var rule1 = mocks.DynamicMock<IRule>();
            var rule2 = mocks.DynamicMock<IRule>();
            var rule3 = mocks.DynamicMock<IRule>();

            using (mocks.Ordered())
            {
                Expect.Call(rule1.Evaluate(response)).Return(Result.Unsuccessful);
                Expect.Call(rule2.Evaluate(response)).Return(Result.Unsuccessful);
                Expect.Call(rule3.Evaluate(response)).Return(new Result(true, MockRepository.GenerateStub<IState>()));
            }
            mocks.ReplayAll();

            var rules = new Rules(rule1, rule2, rule3);
            rules.Evaluate(response);

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldNotEvaluateSubsequentRulesFollowingASuccessfulRule()
        {
            var response = new HttpResponseMessage();

            var rule1 = MockRepository.GenerateMock<IRule>();
            var rule2 = MockRepository.GenerateMock<IRule>();
            var rule3 = MockRepository.GenerateMock<IRule>();

            rule1.Expect(r => r.Evaluate(response)).Return(Result.Unsuccessful);
            rule2.Expect(r => r.Evaluate(response)).Return(new Result(true, MockRepository.GenerateStub<IState>()));

            var rules = new Rules(rule1, rule2, rule3);
            rules.Evaluate(response);

            rule1.VerifyAllExpectations();
            rule2.VerifyAllExpectations();
            rule3.AssertWasNotCalled(r => r.Evaluate(response));
        }
    }
}