using System;
using NUnit.Framework;
using RestInPractice.RestToolkit.Hypermedia;

namespace Tests.RestInPractice.RestToolkit.Hypermedia
{
    [TestFixture]
    public class UriFactoryWorkerTests
    {
        [Test]
        public void ShouldGenerateRelativeUriFromRoutePrefixAndTemplateAndTemplateParameters()
        {
            var uriFactory = new UriFactoryWorker("quotes", "{userId}/{id}");
            Assert.AreEqual("quotes/ian/1", uriFactory.CreateRelativeUri("ian", "1").ToString());
        }

        [Test]
        public void ShouldGenerateAbsoluteUriFromBaseAddressAndRoutePrefixAndTempleAndTemplateParameters()
        {
            var uriFactory = new UriFactoryWorker("quotes", "{userId}/{id}");
            Assert.AreEqual("http://restbucks.com/quotes/ian/1", uriFactory.CreateAbsoluteUri(new Uri("http://restbucks.com"), "ian", "1").ToString());
        }

        [Test]
        public void ShouldUseAllOfTheSuppliedBaseAddressIfTerminatedWithBackslash()
        {
            var uriFactory = new UriFactoryWorker("quotes", "{userId}/{id}");
            Assert.AreEqual("http://restbucks.com:8080/virtual-directory/quotes/ian/1", uriFactory.CreateAbsoluteUri(new Uri("http://restbucks.com:8080/virtual-directory/"), "ian", "1").ToString());
        }

        [Test]
        public void ShouldUseTheSuppliedBaseAddressUpToLastBackslash()
        {
            var uriFactory = new UriFactoryWorker("quotes", "{userId}/{id}");
            Assert.AreEqual("http://restbucks.com:8080/virtual-directory/quotes/ian/1", uriFactory.CreateAbsoluteUri(new Uri("http://restbucks.com:8080/virtual-directory/suffix"), "ian", "1").ToString());
        }

        [Test]
        public void ShouldGenerateRelativeUriWithoutTerminatingBackslashWhenTemplateIsEmpty()
        {
            var uriFactory = new UriFactoryWorker("quotes");
            Assert.AreEqual("quotes", uriFactory.CreateRelativeUri().ToString());
        }

        [Test]
        public void ShouldGenerateRelativeUriWithTerminatingBackslashWhenTemplateEndsWithBackslash()
        {
            var uriFactory = new UriFactoryWorker("quotes", "current/");
            Assert.AreEqual("quotes/current/", uriFactory.CreateRelativeUri().ToString());
        }

        [Test]
        public void ShouldGenerateRelativeUriWithTerminatingBackslashWhenTemplateIsBackslash()
        {
            var uriFactory = new UriFactoryWorker("quotes", "/");
            Assert.AreEqual("quotes/", uriFactory.CreateRelativeUri().ToString());
        }

        [Test]
        public void ShouldGenerateAbsoluteUriWithoutTerminatingBackslashWhenTemplateIsEmpty()
        {
            var uriFactory = new UriFactoryWorker("quotes");
            Assert.AreEqual("http://restbucks.com/quotes", uriFactory.CreateAbsoluteUri(new Uri("http://restbucks.com")).ToString());
        }

        [Test]
        public void ShouldGenerateAbsoluteUriWithTerminatingBackslashWhenTemplateEndsWithBackslash()
        {
            var uriFactory = new UriFactoryWorker("quotes", "current/");
            Assert.AreEqual("http://restbucks.com/quotes/current/", uriFactory.CreateAbsoluteUri(new Uri("http://restbucks.com")).ToString());
        }

        [Test]
        public void ShouldGenerateAbsoluteUriWithTerminatingBackslashWhenTemplateIsBackslash()
        {
            var uriFactory = new UriFactoryWorker("quotes", "/");
            Assert.AreEqual("http://restbucks.com/quotes/", uriFactory.CreateAbsoluteUri(new Uri("http://restbucks.com")).ToString());
        }

        [Test]
        public void ShouldKeepStartingBackslashOnUriTemplateValue()
        {
            var uriFactory = new UriFactoryWorker("orders", "/?a=b");
            Assert.AreEqual("http://restbucks.com/orders/?a=b", uriFactory.CreateAbsoluteUri(new Uri("http://restbucks.com")).ToString());
        }

        [Test]
        public void ShouldGenerateBaseUriWithTerminatingBackslashFromSuppliedAbsoluteUri()
        {
            var uriFactory = new UriFactoryWorker("quotes", "{quoteId}");
            Assert.AreEqual(new Uri("http://restbucks.com:8080/uk/"), uriFactory.CreateBaseUri(new Uri("http://restbucks.com:8080/uk/quotes/1234")));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "Supplied URI does not contain route prefix. Uri: [http://restbucks.com:8080/uk/customers/1234], Route prefix: [quotes].")]
        public void ThrowsExceptionWhenSuppliedUriDoesNotContainRoutePrefix()
        {
            var uriFactory = new UriFactoryWorker("quotes", "{quoteId}");
            uriFactory.CreateBaseUri(new Uri("http://restbucks.com:8080/uk/customers/1234"));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: routePrefix")]
        public void ThrowsExceptionIfRoutePrefixIsNull()
        {
            new UriFactoryWorker(null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "Value cannot be empty.\r\nParameter name: routePrefix")]
        public void ThrowsExceptionIfRoutePrefixIsEmpty()
        {
            new UriFactoryWorker(string.Empty);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "Value cannot be whitespace.\r\nParameter name: routePrefix")]
        public void ThrowsExceptionIfRoutePrefixIsWhitespace()
        {
            new UriFactoryWorker(" ");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: uriTemplateValue")]
        public void ThrowsExceptionIfUriTemplateIsNull()
        {
            new UriFactoryWorker("quotes", null);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException), ExpectedMessage = "Value cannot be whitespace.\r\nParameter name: uriTemplateValue")]
        public void ThrowsExceptionIfUriTemplateValueIsWhitespace()
        {
            new UriFactoryWorker("quotes", " ");
        }
    }
}