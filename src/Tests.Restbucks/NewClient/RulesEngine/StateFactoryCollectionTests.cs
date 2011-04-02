using System.Net;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.NewClient.RulesEngine;
using Rhino.Mocks;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class StateFactoryCollectionTests
    {
        private static readonly HttpResponseMessage Response = new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted};
        private static readonly ApplicationStateVariables StateVariables = new ApplicationStateVariables();
        private static readonly IClientCapabilities DummyClientCapabilities = MockRepository.GenerateStub<IClientCapabilities>();
        private static readonly IState DummyState = MockRepository.GenerateStub<IState>();
        private static readonly ICondition DummyTrueCondition = CreateDummyCondition(true);
        private static readonly ICondition DummyFalseCondition = CreateDummyCondition(false);
        private static readonly IStateFactory DummyStateFactory = MockRepository.GenerateStub<IStateFactory>();

        [Test]
        public void ShouldInvokeCorrectFactoryBasedOnCondition()
        {
            var mockFactory = MockRepository.GenerateMock<IStateFactory>();
            mockFactory.Expect(w => w.Create(Response, StateVariables, DummyClientCapabilities)).Return(DummyState);

            var factoryCollection = new StateFactoryCollection(new[] { new StateCreationRule(DummyTrueCondition, mockFactory.Create) });
            factoryCollection.Create(Response, StateVariables, DummyClientCapabilities);

            mockFactory.VerifyAllExpectations();
        }

        [Test]
        public void ShouldInvokeDefaultFactoryIfNoCOnditionIsSatisfied()
        {
            var mockDefaultFactory = MockRepository.GenerateMock<IStateFactory>();
            mockDefaultFactory.Expect(w => w.Create(Response, StateVariables, DummyClientCapabilities)).Return(DummyState);

            var factoryCollection = new StateFactoryCollection(new[] { new StateCreationRule(DummyFalseCondition, DummyStateFactory.Create) }, mockDefaultFactory.Create);
            factoryCollection.Create(Response, StateVariables, DummyClientCapabilities);

            mockDefaultFactory.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnUnsuccesfulStateIfNoConditionIsSatisfiedAndDefaultFactoryIsNotSupplied()
        {
            var dummyCondition = MockRepository.GenerateStub<ICondition>();
            dummyCondition.Expect(c => c.IsApplicable(Response, StateVariables)).Return(false);

            var factoryCollection = new StateFactoryCollection(new[] { new StateCreationRule(DummyFalseCondition, DummyStateFactory.Create) });
            Assert.IsInstanceOf(typeof (UnsuccessfulState), factoryCollection.Create(Response, StateVariables, DummyClientCapabilities));
        }

        private static ICondition CreateDummyCondition(bool evaluatesTo)
        {
            var dummyCondition = MockRepository.GenerateStub<ICondition>();
            dummyCondition.Expect(c => c.IsApplicable(Response, StateVariables)).Return(evaluatesTo);
            return dummyCondition;
        }
    }
}