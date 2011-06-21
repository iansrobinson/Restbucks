using System.Net;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.RestToolkit.RulesEngine;
using Rhino.Mocks;

namespace Tests.Restbucks.RestToolkit.RulesEngine
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
        private static readonly CreateStateDelegate DummyCreateStateDelegate = (r, v, c) => DummyState;

        [Test]
        public void ShouldInvokeCorrectDelegateBasedOnCondition()
        {
            var factoryCollection = new StateFactoryCollection(new[] { new StateCreationRule(DummyTrueCondition, DummyCreateStateDelegate) });
            var state = factoryCollection.Create(Response, StateVariables, DummyClientCapabilities);

            Assert.AreEqual(DummyState, state);
        }

        [Test]
        public void ShouldInvokeDefaultDelegateIfNoConditionIsSatisfied()
        {
            var defaultState = MockRepository.GenerateStub<IState>();
            CreateStateDelegate createDefaultState = (r, v, c) => defaultState;

            var factoryCollection = new StateFactoryCollection(new[] { new StateCreationRule(DummyFalseCondition, DummyCreateStateDelegate) }, createDefaultState);
            var state = factoryCollection.Create(Response, StateVariables, DummyClientCapabilities);

            Assert.AreEqual(defaultState, state);
        }

        [Test]
        public void ShouldReturnUnsuccesfulStateIfNoConditionIsSatisfiedAndDefaultFactoryIsNotSupplied()
        {
            var dummyCondition = MockRepository.GenerateStub<ICondition>();
            dummyCondition.Expect(c => c.IsApplicable(Response, StateVariables)).Return(false);

            var factoryCollection = new StateFactoryCollection(new[] { new StateCreationRule(DummyFalseCondition, DummyCreateStateDelegate) });
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