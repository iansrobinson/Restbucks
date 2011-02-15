using System;
using NUnit.Framework;
using Restbucks.Client.Keys;

namespace Tests.Restbucks.Client.Keys
{
    [TestFixture]
    public class EntityBodyKeyTests
    {
        [Test]
        public void ValueShouldBeAFunctionOfSuppliedParameters()
        {
            var key = new EntityBodyKey("application/restbucks+xml", "http://schemas.restbucks.com/shop", "context-name");
            Assert.AreEqual("http://context-keys.restbucks.com/media-type:application/restbucks+xml&http://context-keys.restbucks.com/schema:http://schemas.restbucks.com/shop&http://context-keys.restbucks.com/context-name:context-name", key.Value);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: mediaType")]
        public void ThrowsExceptionIfMediaTypeIsNull()
        {
            new EntityBodyKey(null, "http://schema", "context-name");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException), ExpectedMessage = "Value cannot be empty.\r\nParameter name: mediaType")]
        public void ThrowsExceptionIfMediaTypeIsEmpty()
        {
            new EntityBodyKey(string.Empty, "http://schema", "context-name");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException), ExpectedMessage = "Value cannot be whitespace.\r\nParameter name: mediaType")]
        public void ThrowsExceptionIfMediaTypeIsWhitespace()
        {
            new EntityBodyKey(" ", "http://schema", "context-name");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException), ExpectedMessage = "Value cannot be whitespace.\r\nParameter name: schema")]
        public void ThrowsExceptionIfSchemaIsWhitespace()
        {
            new EntityBodyKey("application/restbucks+xml", " ", "context-name");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException), ExpectedMessage = "Value cannot be whitespace.\r\nParameter name: contextName")]
        public void ThrowsExceptionIfContextNameIsWhitespace()
        {
            new EntityBodyKey("application/restbucks+xml", "http://schema", " ");
        }

        [Test]
        public void ShouldAllowNullSchema()
        {
            var key = new EntityBodyKey("application/restbucks+xml", null, "context-name");
            Assert.AreEqual("http://context-keys.restbucks.com/media-type:application/restbucks+xml&http://context-keys.restbucks.com/schema:&http://context-keys.restbucks.com/context-name:context-name", key.Value);
        }

        [Test]
        public void ShouldAllowEMptySchema()
        {
            var key = new EntityBodyKey("application/restbucks+xml", string.Empty, "context-name");
            Assert.AreEqual("http://context-keys.restbucks.com/media-type:application/restbucks+xml&http://context-keys.restbucks.com/schema:&http://context-keys.restbucks.com/context-name:context-name", key.Value);
        }
    }
}