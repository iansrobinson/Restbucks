using System;
using NUnit.Framework;
using Restbucks.MediaType;
using Tests.Restbucks.MediaType.Helpers;

namespace Tests.Restbucks.MediaType
{
    [TestFixture]
    public class FormTests
    {
        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: id")]
        public void ThrowsExceptionIfIdIsNull()
        {
            new Form(null, new Uri("http://localhost"), "post", "application/xml", new Uri("http://localhost"), new ShopBuilder().Build());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException), ExpectedMessage = "Value cannot be empty.\r\nParameter name: id")]
        public void ThrowsExceptionIfIdIsEmpty()
        {
            new Form(string.Empty, new Uri("http://localhost"), "post", "application/xml", new Uri("http://localhost"), new ShopBuilder().Build());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException), ExpectedMessage = "Value cannot be whitespace.\r\nParameter name: id")]
        public void ThrowsExceptionIfIdIsWhitespace()
        {
            new Form(" ", new Uri("http://localhost"), "post", "application/xml", new Uri("http://localhost"), new ShopBuilder().Build());
        }
        
        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: resource")]
        public void ThrowsExceptionIfResourceIsNull()
        {
            new Form("order", null, "post", "application/xml", new Uri("http://localhost"), new ShopBuilder().Build());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: method")]
        public void ThrowsExceptionIfMethodIsNull()
        {
            new Form("order", new Uri("http://localhost"), null, "application/xml", new Uri("http://localhost"), new ShopBuilder().Build());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "Value cannot be whitespace.\r\nParameter name: method")]
        public void ThrowsExceptionIfMethodIsWhitespace()
        {
            new Form("order", new Uri("http://localhost"), " ", "application/xml", new Uri("http://localhost"), new ShopBuilder().Build());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: mediaType")]
        public void ThrowsExceptionIfMediaTypeIsNull()
        {
            new Form("order", new Uri("http://localhost"), "post", null, new Uri("http://localhost"), new ShopBuilder().Build());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "Value cannot be whitespace.\r\nParameter name: mediaType")]
        public void ThrowsExceptionIfMediaTypeIsWhitespace()
        {
            new Form("order", new Uri("http://localhost"), "post", " ", new Uri("http://localhost"), new ShopBuilder().Build());
        }

        [Test]
        public void AllowsNullSchema()
        {
            Assert.IsInstanceOf<Form>(new Form("order", new Uri("http://localhost"), "post", "application/xml", null, new ShopBuilder().Build()));
        }

        [Test]
        public void AllowsNullInstance()
        {
            Assert.IsInstanceOf<Form>(new Form("order", new Uri("http://localhost"), "post", "application/xml", new Uri("http://localhost"), null));
        }
    }
}