using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.NewClient;
using Restbucks.NewClient.RulesEngine;
using Rhino.Mocks;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class WhenTests
    {
        [Test]
        public void ShouldReturnRuleWhoseActionExecutesIfConditionIsTrue()
        {
            var previousResponse = new HttpResponseMessage();

            var action = MockRepository.GenerateMock<IAction>();
            action.Expect(a => a.Execute(previousResponse)).Return(new HttpResponseMessage());

            var rule = When.IsTrue(r => true)
                .ExecuteAction(action)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do(r => null),
                                On.Status(HttpStatusCode.Accepted).Do(r => null)
                            },
                        r => null);

            rule.Evaluate(previousResponse);

            action.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnRuleThatCreatesStateBasedOnResponseStatusCode()
        {
            var previousResponse = new HttpResponseMessage();

            var action = MockRepository.GenerateStub<IAction>();
            action.Expect(a => a.Execute(previousResponse)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var state = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .ExecuteAction(action)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do(r => null),
                                On.Status(HttpStatusCode.Accepted).Do(r => state)
                            },
                        r => null);

            var result = rule.Evaluate(previousResponse);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(state, result.State);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesDefaultStateIfResponseStatusCodeDoesNotMatch()
        {
            var previousResponse = new HttpResponseMessage();

            var action = MockRepository.GenerateStub<IAction>();
            action.Expect(a => a.Execute(previousResponse)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Unauthorized});

            var state = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .ExecuteAction(action)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do(r => null),
                                On.Status(HttpStatusCode.Accepted).Do(r => null)
                            },
                        r => state);

            var result = rule.Evaluate(previousResponse);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(state, result.State);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesUnsuccessfulStateIfResponseStatusCodeDoesNotMatchAndNoDefaultSupplied()
        {
            var previousResponse = new HttpResponseMessage();

            var action = MockRepository.GenerateStub<IAction>();
            action.Expect(a => a.Execute(previousResponse)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Unauthorized});

            var rule = When.IsTrue(r => true)
                .ExecuteAction(action)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do(r => null),
                                On.Status(HttpStatusCode.Accepted).Do(r => null)
                            });

            var result = rule.Evaluate(previousResponse);

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
            var previousResponse = new HttpResponseMessage();

            var action = MockRepository.GenerateStub<IAction>();
            action.Expect(a => a.Execute(previousResponse)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var state = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .ExecuteAction(action)
                .ReturnState(r => state);

            var result = rule.Evaluate(previousResponse);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(state, result.State);
        }

        [Test]
        [Ignore("Some odd things in stub")]
        public void ShouldAllowConditionsToBeUsedToCreateComplexConditions()
        {
            var previousResponse = new HttpResponseMessage();

            var conditions = MockRepository.GenerateStub<IConditions>();
            conditions.Expect(c => c.FormExists(null)).IgnoreArguments().Return(r => true);
            conditions.Expect(c => c.LinkExists(null)).IgnoreArguments().Return(r => true);

            var action = MockRepository.GenerateStub<IAction>();
            action.Expect(a => a.Execute(previousResponse)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var state = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(
                conditions.LinkExists(RestbucksLink.WithRel(new StringLinkRelation("prefetch"))),
                conditions.FormExists(RestbucksForm.WithId("rfq")))
                .ExecuteAction(action)
                .ReturnState(r => state);

            var result = rule.Evaluate(previousResponse);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(state, result.State);
        }
    }
}