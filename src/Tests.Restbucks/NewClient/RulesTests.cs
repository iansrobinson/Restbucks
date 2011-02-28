using System.Net.Http;
using NUnit.Framework;
using Restbucks.NewClient;
using Rhino.Mocks;

namespace Tests.Restbucks.NewClient
{
    [TestFixture]
    public class RulesTests
    {
        [Test]
        public void ShouldCallEachRuleInOrderItWasAddedToRules()
        {
            var response = new HttpResponseMessage();

            var mocks = new MockRepository();

            var rule1 = mocks.StrictMock<IRule>();
            var rule2 = mocks.StrictMock<IRule>();
            var rule3 = mocks.StrictMock<IRule>();

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
    }
}