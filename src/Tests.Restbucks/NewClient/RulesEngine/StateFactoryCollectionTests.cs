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
        private static readonly ApplicationContext Context = new ApplicationContext();
        private static readonly IState DummyState = MockRepository.GenerateStub<IState>();
        private static readonly ICondition DummyTrueCondition = CreateDummyCondition(true);
        private static readonly ICondition DummyFalseCondition = CreateDummyCondition(false);
        private static readonly IStateFactory DummyStateFactory = MockRepository.GenerateStub<IStateFactory>();

        [Test]
        public void ShouldInvokeCorrectFactoryBasedOnCondition()
        {
            var mockFactory = MockRepository.GenerateMock<IStateFactory>();
            mockFactory.Expect(w => w.Create(Response, Context)).Return(DummyState);

            var factoryCollection = new StateFactoryCollection(new[] { new StateCreationRule(DummyTrueCondition, mockFactory) });
            factoryCollection.Create(Response, Context);

            mockFactory.VerifyAllExpectations();
        }

        [Test]
        public void ShouldInvokeDefaultFactoryIfNoCOnditionIsSatisfied()
        {
            var mockDefaultFactory = MockRepository.GenerateMock<IStateFactory>();
            mockDefaultFactory.Expect(w => w.Create(Response, Context)).Return(DummyState);

            var factoryCollection = new StateFactoryCollection(new[] { new StateCreationRule(DummyFalseCondition, DummyStateFactory) }, mockDefaultFactory);
            factoryCollection.Create(Response, Context);

            mockDefaultFactory.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnUnsuccesfulStateIfNoConditionIsSatisfiedAndDefaultFactoryIsNotSupplied()
        {
            var dummyCondition = MockRepository.GenerateStub<ICondition>();
            dummyCondition.Expect(c => c.IsApplicable(Response, Context)).Return(false);

            var factoryCollection = new StateFactoryCollection(new[] { new StateCreationRule(DummyFalseCondition, DummyStateFactory) });
            Assert.IsInstanceOf(typeof (UnsuccessfulState), factoryCollection.Create(Response, Context));
        }

        private static ICondition CreateDummyCondition(bool evaluatesTo)
        {
            var dummyCondition = MockRepository.GenerateStub<ICondition>();
            dummyCondition.Expect(c => c.IsApplicable(Response, Context)).Return(evaluatesTo);
            return dummyCondition;
        }
    }
}