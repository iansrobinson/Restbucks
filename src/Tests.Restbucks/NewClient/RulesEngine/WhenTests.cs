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
            var actionInvoker = MockRepository.GenerateMock<IActionInvoker>();
            actionInvoker.Expect(a => a.Invoke(PreviousResponse, Context)).Return(new HttpResponseMessage());

            var rule = When.IsTrue(r => true)
                .ExecuteAction(actionInvoker)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do((r,c) => null),
                                On.Status(HttpStatusCode.Accepted).Do((r,c) => null)
                            },
                        (r, c) => null);

            rule.Evaluate(PreviousResponse, Context);

            actionInvoker.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnRuleThatCreatesStateBasedOnResponseStatusCode()
        {
            var actionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            actionInvoker.Expect(a => a.Invoke(PreviousResponse, Context)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var state = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .ExecuteAction(actionInvoker)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do((r,c) => null),
                                On.Status(HttpStatusCode.Accepted).Do((r,c) => state)
                            },
                        (r, c) => null);

            var result = rule.Evaluate(PreviousResponse, Context);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(state, result.State);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesDefaultStateIfResponseStatusCodeDoesNotMatch()
        {
            var actionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            actionInvoker.Expect(a => a.Invoke(PreviousResponse, Context)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Unauthorized});

            var state = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .ExecuteAction(actionInvoker)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do((r,c) => null),
                                On.Status(HttpStatusCode.Accepted).Do((r,c) => null)
                            },
                        (r, c) => state);

            var result = rule.Evaluate(PreviousResponse, Context);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(state, result.State);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesUnsuccessfulStateIfResponseStatusCodeDoesNotMatchAndNoDefaultSupplied()
        {
            var actionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            actionInvoker.Expect(a => a.Invoke(PreviousResponse, Context)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Unauthorized});

            var rule = When.IsTrue(r => true)
                .ExecuteAction(actionInvoker)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do((r,c) => null),
                                On.Status(HttpStatusCode.Accepted).Do((r,c) => null)
                            });

            var result = rule.Evaluate(PreviousResponse, Context);

            Assert.IsTrue(result.IsSuccessful);
            Assert.IsInstanceOf(typeof (UnsuccessfulState), result.State);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: createStateByStatusCode")]
        public void ShouldReturnRuleThatCreatesUnsuccessfulStateIfNoStatusCodeMatchersSupplied()
        {
            var actionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            When.IsTrue(r => true)
                .ExecuteAction(actionInvoker)
                .Return(null);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesStateIrrespectiveOfStatusCode()
        {
            var actionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            actionInvoker.Expect(a => a.Invoke(PreviousResponse, Context)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var state = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .ExecuteAction(actionInvoker)
                .ReturnState((r, c) => state);

            var result = rule.Evaluate(PreviousResponse, Context);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(state, result.State);
        }

        [Test]
        public void ShouldAllowComplexConditions()
        {
            var previousResponse = StubResponse.CreateResponse();
            
            var actionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            actionInvoker.Expect(a => a.Invoke(previousResponse, Context)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var state = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => r.ContainsLink(RestbucksLink.WithRel(new StringLinkRelation("http://relations.restbucks.com/rfq")))
                                        && r.ContainsForm(RestbucksForm.WithId("order-form")))
                .ExecuteAction(actionInvoker)
                .ReturnState((r, c) => state);

            var result = rule.Evaluate(previousResponse, Context);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(state, result.State);
        }
    }
}