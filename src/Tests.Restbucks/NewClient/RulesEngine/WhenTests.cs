using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.NewClient;
using Restbucks.NewClient.RulesEngine;
using Rhino.Mocks;
using Tests.Restbucks.NewClient.Util;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class WhenTests
    {
        private static readonly HttpResponseMessage PreviousResponse = new HttpResponseMessage();
        private static readonly ApplicationContext Context = new ApplicationContext();

        [Test]
        public void ShouldReturnRuleWhoseActionExecutesIfConditionIsTrue()
        {
            var mockActionInvoker = MockRepository.GenerateMock<IActionInvoker>();
            mockActionInvoker.Expect(a => a.Invoke(PreviousResponse, Context)).Return(new HttpResponseMessage());

            var rule = When.IsTrue(r => true)
                .ExecuteAction(mockActionInvoker)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do((r, c) => null),
                                On.Status(HttpStatusCode.Accepted).Do((r, c) => null),
                                On.Response(r => r.StatusCode.Is3XX()).Do((r, c) => null),
                                On.Response((r, c) =>
                                            r.StatusCode.Is4XX()
                                            && c.ContainsKey(new StringKey("abort")))
                                    .Do((r, c) => null)
                            },
                        (r, c) => null);

            rule.Evaluate(PreviousResponse, Context);

            mockActionInvoker.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnRuleThatCreatesStateBasedOnResponseStatusCode()
        {
            var dummyActionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            dummyActionInvoker.Expect(a => a.Invoke(PreviousResponse, Context)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var dummyState = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .ExecuteAction(dummyActionInvoker)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do((r, c) => null),
                                On.Status(HttpStatusCode.Accepted).Do((r, c) => dummyState)
                            },
                        (r, c) => null);

            var result = rule.Evaluate(PreviousResponse, Context);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(dummyState, result.State);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesDefaultStateIfResponseStatusCodeDoesNotMatch()
        {
            var dummyActionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            dummyActionInvoker.Expect(a => a.Invoke(PreviousResponse, Context)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Unauthorized});

            var dummyState = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .ExecuteAction(dummyActionInvoker)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do((r, c) => null),
                                On.Status(HttpStatusCode.Accepted).Do((r, c) => null)
                            },
                        (r, c) => dummyState);

            var result = rule.Evaluate(PreviousResponse, Context);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(dummyState, result.State);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesUnsuccessfulStateIfResponseStatusCodeDoesNotMatchAndNoDefaultSupplied()
        {
            var dummyActionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            dummyActionInvoker.Expect(a => a.Invoke(PreviousResponse, Context)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Unauthorized});

            var rule = When.IsTrue(r => true)
                .ExecuteAction(dummyActionInvoker)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do((r, c) => null),
                                On.Status(HttpStatusCode.Accepted).Do((r, c) => null)
                            });

            var result = rule.Evaluate(PreviousResponse, Context);

            Assert.IsTrue(result.IsSuccessful);
            Assert.IsInstanceOf(typeof (UnsuccessfulState), result.State);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: stateCreationRules")]
        public void ShouldThrowExceptionIfStateCreationRulesAreNull()
        {
            var dummyActionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            When.IsTrue(r => true)
                .ExecuteAction(dummyActionInvoker)
                .Return(null);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesUnsuccessfulStateIfNoStatusCodeMatchersSupplied()
        {
            var dummyActionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            var rule = When.IsTrue(r => true)
                .ExecuteAction(dummyActionInvoker)
                .Return(new StateCreationRule[] {});

            var result = rule.Evaluate(PreviousResponse, Context);

            Assert.IsTrue(result.IsSuccessful);
            Assert.IsInstanceOf(typeof (UnsuccessfulState), result.State);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesStateIrrespectiveOfStatusCode()
        {
            var dummyActionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            dummyActionInvoker.Expect(a => a.Invoke(PreviousResponse, Context)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var dummyState = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .ExecuteAction(dummyActionInvoker)
                .ReturnState((r, c) => dummyState);

            var result = rule.Evaluate(PreviousResponse, Context);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(dummyState, result.State);
        }

        [Test]
        public void ShouldAllowComplexConditions()
        {
            var previousResponse = DummyResponse.CreateResponse();

            var dummyActionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            dummyActionInvoker.Expect(a => a.Invoke(previousResponse, Context)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var dummyState = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => r.ContainsLink(RestbucksLink.WithRel(new StringLinkRelation("http://relations.restbucks.com/rfq")))
                                        && r.ContainsForm(RestbucksForm.WithId("request-for-quote")))
                .ExecuteAction(dummyActionInvoker)
                .ReturnState((r, c) => dummyState);

            var result = rule.Evaluate(previousResponse, Context);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(dummyState, result.State);
        }
    }
}