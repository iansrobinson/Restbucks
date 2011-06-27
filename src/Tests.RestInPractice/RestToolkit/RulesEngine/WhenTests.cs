using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.RestToolkit.RulesEngine;
using Rhino.Mocks;
using Tests.RestInPractice.RestToolkit.RulesEngine.Util;

namespace Tests.RestInPractice.RestToolkit.RulesEngine
{
    [TestFixture]
    public class WhenTests
    {
        [Test]
        public void ShouldReturnRuleWhoseActionExecutesIfConditionIsTrue()
        {
            var mockAction = MockRepository.GenerateMock<IRequestAction>();
            mockAction.Expect(a => a.Execute(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities)).Return(new HttpResponseMessage());

            var rule = When.IsTrue(r => true)
                .Execute(actions => actions.Do(mockAction))
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

            rule.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

            mockAction.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnRuleThatCreatesStateBasedOnResponseStatusCode()
        {
            var dummyAction = MockRepository.GenerateStub<IRequestAction>();
            dummyAction.Expect(a => a.Execute(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var dummyState = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .Execute(actions => actions.Do(dummyAction))
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do((r, c) => null),
                                On.Status(HttpStatusCode.Accepted).Do((r, c) => dummyState)
                            },
                        (r, c) => null);

            var result = rule.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(dummyState, result.State);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesDefaultStateIfResponseStatusCodeDoesNotMatch()
        {
            var dummyRequestAction = MockRepository.GenerateStub<IRequestAction>();
            dummyRequestAction.Expect(a => a.Execute(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Unauthorized});

            var dummyState = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .Execute(actions => actions.Do(dummyRequestAction))
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do((r, c) => null),
                                On.Status(HttpStatusCode.Accepted).Do((r, c) => null)
                            },
                        (r, c) => dummyState);

            var result = rule.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(dummyState, result.State);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesUnsuccessfulStateIfResponseStatusCodeDoesNotMatchAndNoDefaultSupplied()
        {
            var dummyRequestAction = MockRepository.GenerateStub<IRequestAction>();
            dummyRequestAction.Expect(a => a.Execute(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Unauthorized});

            var rule = When.IsTrue(r => true)
                .Execute(actions => actions.Do(dummyRequestAction))
                .Return(new[]
                            {
                                On.Status(HttpStatusCode.OK).Do((r, c) => null),
                                On.Status(HttpStatusCode.Accepted).Do((r, c) => null)
                            });

            var result = rule.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

            Assert.IsTrue(result.IsSuccessful);
            Assert.IsInstanceOf(typeof (UnsuccessfulState), result.State);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: stateCreationRules")]
        public void ShouldThrowExceptionIfStateCreationRulesAreNull()
        {
            var dummyRequestAction = MockRepository.GenerateStub<IRequestAction>();
            When.IsTrue(r => true)
                .Execute(actions => actions.Do(dummyRequestAction))
                .Return(null);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesUnsuccessfulStateIfNoStatusCodeMatchersSupplied()
        {
            var dummyRequestAction = MockRepository.GenerateStub<IRequestAction>();
            var rule = When.IsTrue(r => true)
                .Execute(actions => actions.Do(dummyRequestAction))
                .Return(new StateCreationRule[] {});

            var result = rule.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

            Assert.IsTrue(result.IsSuccessful);
            Assert.IsInstanceOf(typeof (UnsuccessfulState), result.State);
        }

        [Test]
        public void ShouldReturnRuleThatCreatesStateIrrespectiveOfStatusCode()
        {
            var dummyRequestAction = MockRepository.GenerateStub<IRequestAction>();
            dummyRequestAction.Expect(a => a.Execute(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var dummyState = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => true)
                .Execute(actions => actions.Do(dummyRequestAction))
                .ReturnState((r, c) => dummyState);

            var result = rule.Evaluate(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(dummyState, result.State);
        }

        [Test]
        public void ShouldAllowComplexConditions()
        {
            var previousResponse = DummyResponse.CreateResponse();

            var dummyRequestAction = MockRepository.GenerateStub<IRequestAction>();
            dummyRequestAction.Expect(a => a.Execute(Dummy.PreviousResponse, Dummy.StateVariables, Dummy.ClientCapabilities)).Return(new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted});

            var dummyState = MockRepository.GenerateStub<IState>();

            var rule = When.IsTrue(r => r.ContainsLink(new DummyLinkStrategy(DummyResponse.LinkRel))
                                        && r.ContainsForm(new DummyFormStrategy(DummyResponse.FormId)))
                .Execute(actions => actions.Do(dummyRequestAction))
                .ReturnState((r, c) => dummyState);

            var result = rule.Evaluate(previousResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(dummyState, result.State);
        }
    }
}