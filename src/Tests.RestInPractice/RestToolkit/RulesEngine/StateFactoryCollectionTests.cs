using NUnit.Framework;
using Restbucks.RestToolkit.RulesEngine;
using Rhino.Mocks;
using Tests.RestInPractice.RestToolkit.RulesEngine.Util;

namespace Tests.RestInPractice.RestToolkit.RulesEngine
{
    [TestFixture]
    public class StateFactoryCollectionTests
    {
        [Test]
        public void ShouldInvokeCorrectDelegateBasedOnCondition()
        {
            var factoryCollection = new StateFactoryCollection(new[] {new StateCreationRule(Dummy.Condition(true), Dummy.CreateNextState())});
            var state = factoryCollection.Execute(Dummy.NewResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

            Assert.AreEqual(Dummy.NewState, state);
        }

        [Test]
        public void ShouldInvokeDefaultDelegateIfNoConditionIsSatisfied()
        {
            var defaultState = MockRepository.GenerateStub<IState>();
            CreateStateDelegate createDefaultState = (r, v, c) => defaultState;

            var factoryCollection = new StateFactoryCollection(new[] {new StateCreationRule(Dummy.Condition(false), Dummy.CreateNextState())}, createDefaultState);
            var state = factoryCollection.Execute(Dummy.NewResponse, Dummy.StateVariables, Dummy.ClientCapabilities);

            Assert.AreEqual(defaultState, state);
        }

        [Test]
        public void ShouldReturnUnsuccesfulStateIfNoConditionIsSatisfiedAndDefaultFactoryIsNotSupplied()
        {
            var dummyCondition = MockRepository.GenerateStub<ICondition>();
            dummyCondition.Expect(c => c.IsApplicable(Dummy.NewResponse, Dummy.StateVariables)).Return(false);

            var factoryCollection = new StateFactoryCollection(new[] {new StateCreationRule(Dummy.Condition(false), Dummy.CreateNextState())});
            Assert.IsInstanceOf(typeof (UnsuccessfulState), factoryCollection.Execute(Dummy.NewResponse, Dummy.StateVariables, Dummy.ClientCapabilities));
        }
    }
}