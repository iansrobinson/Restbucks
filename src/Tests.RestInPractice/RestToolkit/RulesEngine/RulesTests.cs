using NUnit.Framework;
using RestInPractice.RestToolkit.RulesEngine;
using Rhino.Mocks;
using Tests.RestInPractice.RestToolkit.RulesEngine.Util;

namespace Tests.RestInPractice.RestToolkit.RulesEngine
{
    [TestFixture]
    public class RulesTests
    {
        [Test]
        public void ShouldCallEachRuleInOrderItWasAddedToRules()
        {
            var mocks = new MockRepository();

            var mockRule1 = mocks.DynamicMock<IRule>();
            var mockRule2 = mocks.DynamicMock<IRule>();
            var mockRule3 = mocks.DynamicMock<IRule>();

            using (mocks.Ordered())
            {
                Expect.Call(mockRule1.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities)).Return(Result.Unsuccessful);
                Expect.Call(mockRule2.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities)).Return(Result.Unsuccessful);
                Expect.Call(mockRule3.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities)).Return(new Result(true, MockRepository.GenerateStub<IState>()));
            }
            mocks.ReplayAll();

            var rules = new Rules(mockRule1, mockRule2, mockRule3);
            rules.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldNotEvaluateSubsequentRulesFollowingASuccessfulRule()
        {
            var mockRule1 = MockRepository.GenerateMock<IRule>();
            var mockRule2 = MockRepository.GenerateMock<IRule>();
            var mockRule3 = MockRepository.GenerateMock<IRule>();

            mockRule1.Expect(r => r.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities)).Return(Result.Unsuccessful);
            mockRule2.Expect(r => r.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities)).Return(new Result(true, MockRepository.GenerateStub<IState>()));

            var rules = new Rules(mockRule1, mockRule2, mockRule3);
            rules.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

            mockRule1.VerifyAllExpectations();
            mockRule2.VerifyAllExpectations();
            mockRule3.AssertWasNotCalled(r => r.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities));
        }
    }
}