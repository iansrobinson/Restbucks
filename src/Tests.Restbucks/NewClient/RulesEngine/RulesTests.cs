﻿using System.Net.Http;
using NUnit.Framework;
using Restbucks.NewClient.RulesEngine;
using Rhino.Mocks;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class RulesTests
    {
        private static readonly HttpResponseMessage Response = new HttpResponseMessage();
        private static readonly ApplicationStateVariables StateVariables = new ApplicationStateVariables();

        [Test]
        public void ShouldCallEachRuleInOrderItWasAddedToRules()
        {
            var mocks = new MockRepository();

            var mockRule1 = mocks.DynamicMock<IRule>();
            var mockRule2 = mocks.DynamicMock<IRule>();
            var mockRule3 = mocks.DynamicMock<IRule>();

            using (mocks.Ordered())
            {
                Expect.Call(mockRule1.Evaluate(Response, StateVariables)).Return(Result.Unsuccessful);
                Expect.Call(mockRule2.Evaluate(Response, StateVariables)).Return(Result.Unsuccessful);
                Expect.Call(mockRule3.Evaluate(Response, StateVariables)).Return(new Result(true, MockRepository.GenerateStub<IState>()));
            }
            mocks.ReplayAll();

            var rules = new Rules(mockRule1, mockRule2, mockRule3);
            rules.Evaluate(Response, StateVariables);

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldNotEvaluateSubsequentRulesFollowingASuccessfulRule()
        {
            var mockRule1 = MockRepository.GenerateMock<IRule>();
            var mockRule2 = MockRepository.GenerateMock<IRule>();
            var mockRule3 = MockRepository.GenerateMock<IRule>();

            mockRule1.Expect(r => r.Evaluate(Response, StateVariables)).Return(Result.Unsuccessful);
            mockRule2.Expect(r => r.Evaluate(Response, StateVariables)).Return(new Result(true, MockRepository.GenerateStub<IState>()));

            var rules = new Rules(mockRule1, mockRule2, mockRule3);
            rules.Evaluate(Response, StateVariables);

            mockRule1.VerifyAllExpectations();
            mockRule2.VerifyAllExpectations();
            mockRule3.AssertWasNotCalled(r => r.Evaluate(Response, StateVariables));
        }
    }
}