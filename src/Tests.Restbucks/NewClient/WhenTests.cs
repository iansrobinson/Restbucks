using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.NewClient;
using Rhino.Mocks;

namespace Tests.Restbucks.NewClient
{
    [TestFixture]
    public class WhenTests
    {
        [Test]
        public void ShouldReturnRuleWhoseActionExecutesIfConditionIsTrue()
        {
            var action = MockRepository.GenerateMock<IAction>();
            action.Expect(a => a.Execute()).Return(new HttpResponseMessage());

            var rule = When.IsTrue(r => true)
                .ExecuteAction(action)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do(r => null),
                                On.Status(HttpStatusCode.Accepted).Do(r => null)
                            },
                        r => null);

            rule.Evaluate(new HttpResponseMessage());

            action.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnRuleThatCreatesStateBasedOnResponseStatusCode()
        {
            var action = MockRepository.GenerateStub<IAction>();
            action.Expect(a => a.Execute()).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var state = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .ExecuteAction(action)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do(r => null),
                                On.Status(HttpStatusCode.Accepted).Do(r => state)
                            },
                        r => null);

            var result = rule.Evaluate(new HttpResponseMessage());

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(state, result.State);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesDefaultStateIfResponseStatusCodeDoesNotMatch()
        {
            var action = MockRepository.GenerateStub<IAction>();
            action.Expect(a => a.Execute()).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Unauthorized});

            var state = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .ExecuteAction(action)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do(r => null),
                                On.Status(HttpStatusCode.Accepted).Do(r => null)
                            },
                        r => state);

            var result = rule.Evaluate(new HttpResponseMessage());

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(state, result.State);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesUnsuccessfulStateIfResponseStatusCodeDoesNotMatchAndNoDefaultSupplied()
        {
            var action = MockRepository.GenerateStub<IAction>();
            action.Expect(a => a.Execute()).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Unauthorized});

            var state = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .ExecuteAction(action)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do(r => null),
                                On.Status(HttpStatusCode.Accepted).Do(r => null)
                            });

            var result = rule.Evaluate(new HttpResponseMessage());

            Assert.IsTrue(result.IsSuccessful);
            Assert.IsInstanceOf(typeof (UnsuccessfulState), result.State);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: createStateByStatusCode")]
        public void ShouldReturnRuleThatCreatesUnsuccessfulStateIfNoStatusCodeMatchersSupplied()
        {
            var action = MockRepository.GenerateStub<IAction>();
            When.IsTrue(r => true)
                .ExecuteAction(action)
                .Return(null);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesStateIrrespectiveOfStatusCode()
        {
            var action = MockRepository.GenerateStub<IAction>();
            action.Expect(a => a.Execute()).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var state = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .ExecuteAction(action)
                .ReturnState(r => state);

            var result = rule.Evaluate(new HttpResponseMessage());

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(state, result.State);
        }
    }
}