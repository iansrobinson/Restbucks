using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.NewClient.RulesEngine;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class ApplicationContextTests
    {
        private static readonly ExampleObject FirstValue = new ExampleObject();
        private static readonly ExampleObject SecondValue = new ExampleObject();

        private static readonly ApplicationContext Context = new ApplicationContext(
            new KeyValuePair<IKey, object>(new StringKey("key"), FirstValue),
            new KeyValuePair<IKey, object>(new EntityBodyKey("order-form", new MediaTypeHeaderValue(RestbucksMediaType.Value), new Uri("http://schemas/shop")), SecondValue));

        [Test]
        public void ShouldReturnDataByKey()
        {
            Assert.AreEqual(FirstValue, Context.Get<ExampleObject>(new StringKey("key")));
            Assert.AreEqual(SecondValue, Context.Get<ExampleObject>(new EntityBodyKey("order-form", new MediaTypeHeaderValue(RestbucksMediaType.Value), new Uri("http://schemas/shop"))));
        }

        [Test]
        public void ShouldReturnTrueIfKeyExists()
        {
            Assert.IsTrue(Context.ContainsKey(new StringKey("key")));
            Assert.IsTrue(Context.ContainsKey(new EntityBodyKey("order-form", new MediaTypeHeaderValue(RestbucksMediaType.Value), new Uri("http://schemas/shop"))));
        }

        [Test]
        public void ShouldReturnFalseIfKeyDoesNotExist()
        {
            Assert.IsFalse(Context.ContainsKey(new StringKey("not-a-key")));
            Assert.IsFalse(Context.ContainsKey(new EntityBodyKey("different-id", new MediaTypeHeaderValue(RestbucksMediaType.Value), new Uri("http://schemas/shop"))));

        }

        [Test]
        public void ShouldAllowNewValueToBeAdded()
        {
            var newValue = new object();
            var newContext = ApplicationContext.GetBuilder(Context).Add(new StringKey("new-key"), newValue).Build();

            Assert.AreEqual(newValue, newContext.Get<object>(new StringKey("new-key")));
        }

        [Test]
        public void ShouldReturnNewContextWhenAddingValue()
        {
            var newContext = ApplicationContext.GetBuilder(Context).Add(new StringKey("new-key"), new object()).Build();
            Assert.AreNotEqual(Context, newContext);
        }

        [Test]
        public void NewValueShouldNotBeAddedToOriginalContext()
        {
            ApplicationContext.GetBuilder(Context).Add(new StringKey("new-key"), new object()).Build();
            Assert.IsFalse(Context.ContainsKey(new StringKey("new-key")));
        }

        [Test]
        [ExpectedException(typeof (InvalidCastException))]
        public void ThrowsExceptionIfReturnValueIsDifferentTypeFromGenericParameter()
        {
            Context.Get<DifferentObject>(new StringKey("key"));
        }

        private class ExampleObject
        {
        }

        private class DifferentObject
        {
        }
    }
}