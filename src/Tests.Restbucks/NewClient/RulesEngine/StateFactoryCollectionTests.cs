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
        private static readonly IState DummyState = MockRepository.GenerateStub<IState>();
        private static readonly IsApplicableToStateInfoDelegate DummyTrueCondition = (response, variables) => true;
        private static readonly IsApplicableToStateInfoDelegate DummyFalseCondition = (response, variables) => false;
        private static readonly IStateFactory DummyStateFactory = MockRepository.GenerateStub<IStateFactory>();

        [Test]
        public void ShouldInvokeCorrectFactoryBasedOnCondition()
        {
            var mockFactory = MockRepository.GenerateMock<IStateFactory>();
            mockFactory.Expect(w => w.Create(Response, StateVariables)).Return(DummyState);

            var factoryCollection = new StateFactoryCollection(new[] { new StateCreationRule(DummyTrueCondition, mockFactory) });
            factoryCollection.Create(Response, StateVariables);

            mockFactory.VerifyAllExpectations();
        }

        [Test]
        public void ShouldInvokeDefaultFactoryIfNoCOnditionIsSatisfied()
        {
            var mockDefaultFactory = MockRepository.GenerateMock<IStateFactory>();
            mockDefaultFactory.Expect(w => w.Create(Response, StateVariables)).Return(DummyState);

            var factoryCollection = new StateFactoryCollection(new[] { new StateCreationRule(DummyFalseCondition, DummyStateFactory) }, mockDefaultFactory);
            factoryCollection.Create(Response, StateVariables);

            mockDefaultFactory.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnUnsuccesfulStateIfNoConditionIsSatisfiedAndDefaultFactoryIsNotSupplied()
        {
            var dummyCondition = MockRepository.GenerateStub<ICondition>();
            dummyCondition.Expect(c => c.IsApplicable(Response, StateVariables)).Return(false);

            var factoryCollection = new StateFactoryCollection(new[] { new StateCreationRule(DummyFalseCondition, DummyStateFactory) });
            Assert.IsInstanceOf(typeof (UnsuccessfulState), factoryCollection.Create(Response, StateVariables));
        }
    }
}