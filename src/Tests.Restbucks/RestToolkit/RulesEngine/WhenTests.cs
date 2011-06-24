﻿using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.Client.Hypermedia.Strategies;
using Restbucks.MediaType;
using Restbucks.Client;
using Restbucks.RestToolkit.RulesEngine;
using Rhino.Mocks;
using Tests.Restbucks.Client.Util;

namespace Tests.Restbucks.RestToolkit.RulesEngine
{
    [TestFixture]
    public class WhenTests
    {
        private static readonly HttpResponseMessage PreviousResponse = new HttpResponseMessage();
        private static readonly ApplicationStateVariables StateVariables = new ApplicationStateVariables();
        private static readonly IClientCapabilities DummyClientCapabilities = MockRepository.GenerateStub<IClientCapabilities>();

        [Test]
        public void ShouldReturnRuleWhoseActionExecutesIfConditionIsTrue()
        {
            var mockAction = MockRepository.GenerateMock<IGenerateNextRequest>();
            mockAction.Expect(a => a.Execute(PreviousResponse, StateVariables, DummyClientCapabilities)).Return(new HttpResponseMessage());

            var rule = When.IsTrue(r => true)
                .Invoke(actions => actions.Do(mockAction))
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

            rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            mockAction.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnRuleThatCreatesStateBasedOnResponseStatusCode()
        {
            var dummyAction = MockRepository.GenerateStub<IGenerateNextRequest>();
            dummyAction.Expect(a => a.Execute(PreviousResponse, StateVariables, DummyClientCapabilities)).Return(new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted });

            var dummyState = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .Invoke(actions => actions.Do(dummyAction))
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do((r, c) => null),
                                On.Status(HttpStatusCode.Accepted).Do((r, c) => dummyState)
                            },
                        (r, c) => null);

            var result = rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(dummyState, result.State);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesDefaultStateIfResponseStatusCodeDoesNotMatch()
        {
            var dummyAction = MockRepository.GenerateStub<IGenerateNextRequest>();
            dummyAction.Expect(a => a.Execute(PreviousResponse, StateVariables, DummyClientCapabilities)).Return(new HttpResponseMessage { StatusCode = HttpStatusCode.Unauthorized });

            var dummyState = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .Invoke(actions => actions.Do(dummyAction))
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do((r, c) => null),
                                On.Status(HttpStatusCode.Accepted).Do((r, c) => null)
                            },
                        (r, c) => dummyState);

            var result = rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(dummyState, result.State);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesUnsuccessfulStateIfResponseStatusCodeDoesNotMatchAndNoDefaultSupplied()
        {
            var dummyAction = MockRepository.GenerateStub<IGenerateNextRequest>();
            dummyAction.Expect(a => a.Execute(PreviousResponse, StateVariables, DummyClientCapabilities)).Return(new HttpResponseMessage { StatusCode = HttpStatusCode.Unauthorized });

            var rule = When.IsTrue(r => true)
                .Invoke(actions => actions.Do(dummyAction))
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do((r, c) => null),
                                On.Status(HttpStatusCode.Accepted).Do((r, c) => null)
                            });

            var result = rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            Assert.IsTrue(result.IsSuccessful);
            Assert.IsInstanceOf(typeof (UnsuccessfulState), result.State);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: stateCreationRules")]
        public void ShouldThrowExceptionIfStateCreationRulesAreNull()
        {
            var dummyAction = MockRepository.GenerateStub<IGenerateNextRequest>(); 
            When.IsTrue(r => true)
                .Invoke(actions => actions.Do(dummyAction))
                .Return(null);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesUnsuccessfulStateIfNoStatusCodeMatchersSupplied()
        {
            var dummyAction = MockRepository.GenerateStub<IGenerateNextRequest>();
            var rule = When.IsTrue(r => true)
                .Invoke(actions => actions.Do(dummyAction))
                .Return(new StateCreationRule[] {});

            var result = rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            Assert.IsTrue(result.IsSuccessful);
            Assert.IsInstanceOf(typeof (UnsuccessfulState), result.State);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesStateIrrespectiveOfStatusCode()
        {
            var dummyAction = MockRepository.GenerateStub<IGenerateNextRequest>();
            dummyAction.Expect(a => a.Execute(PreviousResponse, StateVariables, DummyClientCapabilities)).Return(new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted });

            var dummyState = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .Invoke(actions => actions.Do(dummyAction))
                .ReturnState((r, c) => dummyState);

            var result = rule.Evaluate(PreviousResponse, StateVariables, DummyClientCapabilities);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(dummyState, result.State);
        }

        [Test]
        public void ShouldAllowComplexConditions()
        {
            var previousResponse = DummyResponse.CreateResponse();

            var dummyAction = MockRepository.GenerateStub<IGenerateNextRequest>();
            dummyAction.Expect(a => a.Execute(PreviousResponse, StateVariables, DummyClientCapabilities)).Return(new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted });

            var dummyState = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => r.ContainsLink(RestbucksLink.WithRel(new StringLinkRelation("http://relations.restbucks.com/rfq")))
                                        && r.ContainsForm(RestbucksForm.WithId("request-for-quote")))
                .Invoke(actions => actions.Do(dummyAction))
                .ReturnState((r, c) => dummyState);

            var result = rule.Evaluate(previousResponse, StateVariables, DummyClientCapabilities);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(dummyState, result.State);
        }
    }
}