using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.Client.Formatters;
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

            var actionInvoker = MockRepository.GenerateMock<IActionInvoker>();
            actionInvoker.Expect(a => a.Invoke(previousResponse)).Return(new HttpResponseMessage());

            var rule = When.IsTrue(r => true)
                .ExecuteAction(actionInvoker)
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do(r => null),
                                On.Status(HttpStatusCode.Accepted).Do(r => null)
                            },
                        r => null);

            rule.Evaluate(previousResponse);

            actionInvoker.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnRuleThatCreatesStateBasedOnResponseStatusCode()
        {
            var previousResponse = new HttpResponseMessage();

            var actionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            actionInvoker.Expect(a => a.Invoke(previousResponse)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var state = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .ExecuteAction(actionInvoker)
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

            var actionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            actionInvoker.Expect(a => a.Invoke(previousResponse)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Unauthorized});

            var state = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .ExecuteAction(actionInvoker)
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

            var actionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            actionInvoker.Expect(a => a.Invoke(previousResponse)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Unauthorized});

            var rule = When.IsTrue(r => true)
                .ExecuteAction(actionInvoker)
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
            var actionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            When.IsTrue(r => true)
                .ExecuteAction(actionInvoker)
                .Return(null);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesStateIrrespectiveOfStatusCode()
        {
            var previousResponse = new HttpResponseMessage();

            var actionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            actionInvoker.Expect(a => a.Invoke(previousResponse)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var state = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .ExecuteAction(actionInvoker)
                .ReturnState(r => state);

            var result = rule.Evaluate(previousResponse);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(state, result.State);
        }

        [Test]
        public void ShouldAllowComplexConditions()
        {
            var previousResponse = CreateResponse();

            var actionInvoker = MockRepository.GenerateStub<IActionInvoker>();
            actionInvoker.Expect(a => a.Invoke(previousResponse)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var state = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => r.ContainsLink(RestbucksLink.WithRel(new StringLinkRelation("prefetch")))
                                        && r.ContainsForm(RestbucksForm.WithId("order-form")))
                .ExecuteAction(actionInvoker)
                .ReturnState(r => state);

            var result = rule.Evaluate(previousResponse);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(state, result.State);
        }

        private static HttpResponseMessage CreateResponse()
        {
            var entityBody = new ShopBuilder(new Uri("http://localhost/virtual-directory/"))
                .AddLink(new Link(
                             new Uri("request-for-quote", UriKind.Relative),
                             RestbucksMediaType.Value,
                             new StringLinkRelation("prefetch")))
                .AddForm(new Form(
                             "order-form",
                             new Uri("orders", UriKind.Relative),
                             "post",
                             RestbucksMediaType.Value,
                             null as Shop))
                .Build();

            var content = entityBody.ToContent(RestbucksMediaTypeFormatter.Instance);
            content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);

            return new HttpResponseMessage {Content = content};
        }
    }
}