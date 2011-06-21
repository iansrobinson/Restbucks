using System.Net.Http;
using NUnit.Framework;
using Restbucks.RestToolkit.RulesEngine;
using Rhino.Mocks;

namespace Tests.Restbucks.RestToolkit.RulesEngine
{
    [TestFixture]
    public class RulesTests
    {
        private static readonly HttpResponseMessage Response = new HttpResponseMessage();
        private static readonly ApplicationStateVariables StateVariables = new ApplicationStateVariables();
        private static readonly IClientCapabilities DummyClientCapabilities = MockRepository.GenerateStub<IClientCapabilities>();

        [Test]
        public void ShouldCallEachRuleInOrderItWasAddedToRules()
        {
            var mocks = new MockRepository();

            var mockRule1 = mocks.DynamicMock<IRule>();
            var mockRule2 = mocks.DynamicMock<IRule>();
            var mockRule3 = mocks.DynamicMock<IRule>();

            using (mocks.Ordered())
            {
                Expect.Call(mockRule1.Evaluate(Response, StateVariables, DummyClientCapabilities)).Return(Result.Unsuccessful);
                Expect.Call(mockRule2.Evaluate(Response, StateVariables, DummyClientCapabilities)).Return(Result.Unsuccessful);
                Expect.Call(mockRule3.Evaluate(Response, StateVariables, DummyClientCapabilities)).Return(new Result(true, MockRepository.GenerateStub<IState>()));
            }
            mocks.ReplayAll();

            var rules = new Rules(mockRule1, mockRule2, mockRule3);
            rules.Evaluate(Response, StateVariables, DummyClientCapabilities);

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldNotEvaluateSubsequentRulesFollowingASuccessfulRule()
        {
            var mockRule1 = MockRepository.GenerateMock<IRule>();
            var mockRule2 = MockRepository.GenerateMock<IRule>();
            var mockRule3 = MockRepository.GenerateMock<IRule>();

            mockRule1.Expect(r => r.Evaluate(Response, StateVariables, DummyClientCapabilities)).Return(Result.Unsuccessful);
            mockRule2.Expect(r => r.Evaluate(Response, StateVariables, DummyClientCapabilities)).Return(new Result(true, MockRepository.GenerateStub<IState>()));

            var rules = new Rules(mockRule1, mockRule2, mockRule3);
            rules.Evaluate(Response, StateVariables, DummyClientCapabilities);

            mockRule1.VerifyAllExpectations();
            mockRule2.VerifyAllExpectations();
            mockRule3.AssertWasNotCalled(r => r.Evaluate(Response, StateVariables, DummyClientCapabilities));
        }
    }
}