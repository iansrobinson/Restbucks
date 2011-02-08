using System;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.Keys;

namespace Tests.Restbucks.Client
{
    [TestFixture]
    public class ApplicationContextTests
    {
        [Test]
        public void CanAddAndRetrieveTypedObject()
        {
            var value = new ExampleObject();

            var context = new ApplicationContext();
            
            context.Set(new StringKey("key-name"), value);
            var returnedObject = context.Get<ExampleObject>(new StringKey("key-name"));

            Assert.AreEqual(value, returnedObject);
        }

        [Test]
        [ExpectedException(typeof(InvalidCastException))]
        public void ThrowsExceptionIfReturnValueIsDifferentTypeFromGenericParameter()
        {
            var value = new ExampleObject();

            var context = new ApplicationContext();

            context.Set(new StringKey("key-name"), value);
            context.Get<DifferentObject>(new StringKey("key-name"));
        }

        [Test]
        public void ShouldOverwriteExistingValueWithNewValueIfKeyIsSame()
        {
            var value1 = new ExampleObject();
            var value2 = new ExampleObject();

            var context = new ApplicationContext();

            context.Set(new StringKey("key-name"), value1);
            context.Set(new StringKey("key-name"), value2);
            var returnedObject = context.Get<ExampleObject>(new StringKey("key-name"));

            Assert.AreEqual(value2, returnedObject);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(InvalidOperationException), ExpectedMessage = "Unable to replace an existing value with a value of a different type.")]
        public void ThrowsExceptionIfTryingToReplaceValueWithValueOfDifferentType()
        {
            var value1 = new ExampleObject();
            var value2 = new DifferentObject();

            var context = new ApplicationContext();

            context.Set(new StringKey("key-name"), value1);
            context.Set(new StringKey("key-name"), value2);
        }

        private class ExampleObject
        {
        }

        private class DifferentObject
        {
            
        }
    }
}