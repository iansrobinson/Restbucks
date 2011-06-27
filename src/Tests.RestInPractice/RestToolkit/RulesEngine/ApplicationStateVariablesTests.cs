using System;
using System.Collections.Generic;
using NUnit.Framework;
using Restbucks.RestToolkit.RulesEngine;
using Tests.RestInPractice.RestToolkit.Utils;

namespace Tests.RestInPractice.RestToolkit.RulesEngine
{
    [TestFixture]
    public class ApplicationStateVariablesTests
    {
        private static readonly ExampleObject FirstValue = new ExampleObject();
        private static readonly ExampleObject SecondValue = new ExampleObject();

        private static readonly ApplicationStateVariables StateVariables = new ApplicationStateVariables(
            new KeyValuePair<IKey, object>(new StringKey("key"), FirstValue),
            new KeyValuePair<IKey, object>(new EntityBodyKey("order-form", DummyMediaType.ContentType, new Uri("http://schemas/shop")), SecondValue));

        [Test]
        public void ShouldReturnDataByKey()
        {
            Assert.AreEqual(FirstValue, StateVariables.Get<ExampleObject>(new StringKey("key")));
            Assert.AreEqual(SecondValue, StateVariables.Get<ExampleObject>(new EntityBodyKey("order-form", DummyMediaType.ContentType, new Uri("http://schemas/shop"))));
        }

        [Test]
        public void ShouldReturnTrueIfKeyExists()
        {
            Assert.IsTrue(StateVariables.ContainsKey(new StringKey("key")));
            Assert.IsTrue(StateVariables.ContainsKey(new EntityBodyKey("order-form", DummyMediaType.ContentType, new Uri("http://schemas/shop"))));
        }

        [Test]
        public void ShouldReturnFalseIfKeyDoesNotExist()
        {
            Assert.IsFalse(StateVariables.ContainsKey(new StringKey("not-a-key")));
            Assert.IsFalse(StateVariables.ContainsKey(new EntityBodyKey("different-id", DummyMediaType.ContentType, new Uri("http://schemas/shop"))));
        }

        [Test]
        public void ShouldAllowNewValueToBeAdded()
        {
            var newValue = new object();
            var newContext = StateVariables.GetNewStateVariablesBuilder().Add(new StringKey("new-key"), newValue).Build();

            Assert.AreEqual(newValue, newContext.Get<object>(new StringKey("new-key")));
        }

        [Test]
        public void ShouldReturnNewContextAfterBuilding()
        {
            var newContext = StateVariables.GetNewStateVariablesBuilder().Add(new StringKey("new-key"), new object()).Build();
            Assert.AreNotEqual(StateVariables, newContext);
        }

        [Test]
        public void NewValueShouldNotBeAddedToOriginalContext()
        {
            StateVariables.GetNewStateVariablesBuilder().Add(new StringKey("new-key"), new object()).Build();
            Assert.IsFalse(StateVariables.ContainsKey(new StringKey("new-key")));
        }

        [Test]
        public void ShouldNotAllowNewValueToBeAddedToNewContextOnceBuildHasBeenCalled()
        {
            var builder = StateVariables.GetNewStateVariablesBuilder();
            var newContext = builder.Build();
            builder.Add(new StringKey("new-key"), new object());

            Assert.IsFalse(newContext.ContainsKey(new StringKey("new-key")));
        }

        [Test]
        public void ShouldAllowKeyToBeRemoved()
        {
            var newContext = StateVariables.GetNewStateVariablesBuilder().Remove(new StringKey("key")).Build();
            Assert.IsFalse(newContext.ContainsKey(new StringKey("key")));
        }

        [Test]
        public void ValueShouldNotBeRemovedFromOriginalContext()
        {
            StateVariables.GetNewStateVariablesBuilder().Remove(new StringKey("key")).Build();
            Assert.IsTrue(StateVariables.ContainsKey(new StringKey("key")));
        }

        [Test]
        public void ShouldAllowValueToBeUpdated()
        {
            var newValue = new ExampleObject();
            var newContext = StateVariables.GetNewStateVariablesBuilder().Update(new StringKey("key"), newValue).Build();

            Assert.AreEqual(newValue, newContext.Get<ExampleObject>(new StringKey("key")));
        }

        [Test]
        public void ValueShouldNotBeUpdatedInOriginalContext()
        {
            var newValue = new ExampleObject();
            StateVariables.GetNewStateVariablesBuilder().Update(new StringKey("key"), newValue).Build();

            Assert.AreNotEqual(newValue, StateVariables.Get<ExampleObject>(new StringKey("key")));
            Assert.AreEqual(FirstValue, StateVariables.Get<ExampleObject>(new StringKey("key")));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (InvalidOperationException), ExpectedMessage = "Unable to replace an existing value with a value of a different type.")]
        public void ThrowsExceptionIfTryingToUpdateValueWithValueOfDifferentType()
        {
            StateVariables.GetNewStateVariablesBuilder().Update(new StringKey("key"), new DifferentObject()).Build();
        }

        [Test]
        [ExpectedException(typeof (InvalidCastException))]
        public void ThrowsExceptionIfReturnValueIsDifferentTypeFromGenericParameter()
        {
            StateVariables.Get<DifferentObject>(new StringKey("key"));
        }

        private class ExampleObject
        {
        }

        private class DifferentObject
        {
        }
    }
}