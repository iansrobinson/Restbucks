using System;
using NUnit.Framework;
using Restbucks.MediaType;

namespace Tests.Restbucks.MediaType
{
    [TestFixture]
    public class HrefTests
    {
        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: uri")]
        public void ThrowsExceptionIfUriIsNull()
        {
            new Href(null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: accessUri")]
        public void ThrowsExceptionIfAccessUriIsNull()
        {
            new Href(new Uri("http://localhost"), null);
        }

        [Test]
        public void ShouldBeDereferenceableIfAccessUriIsAbsoluteUri()
        {
            var href = new Href(new Uri("http://localhost"), new Uri("http://localhost"));
            Assert.IsTrue(href.IsDereferenceable);
        }

        [Test]
        public void ShouldNotBeDereferenceableIfAccessUriIsNotAbsoluteUri()
        {
            var href = new Href(new Uri("http://localhost"), new Uri("/quotes", UriKind.Relative));
            Assert.IsFalse(href.IsDereferenceable);
        }

        [Test]
        public void ShouldBeDereferenceableIfUriIsAbsoluteUri()
        {
            var href = new Href(new Uri("http://localhost"));
            Assert.IsTrue(href.IsDereferenceable);
        }

        [Test]
        public void ShouldNotBeDereferenceableIfAccessIsNotAbsoluteUri()
        {
            var href = new Href(new Uri("/quotes", UriKind.Relative));
            Assert.IsFalse(href.IsDereferenceable);
        }
    }
}