using System;
using NUnit.Framework;
using Restbucks.Quoting.Service.Resources;

namespace Tests.Restbucks.Quoting.Service.Resources
{
    [TestFixture]
    public class UriFactoryWorkerTests
    {
        [Test]
        public void ShouldGenerateRelativeUriFromRoutePrefixAndTemplateAndTemplateParameters()
        {
            var uriFactory = new UriFactoryWorker("quotes", "{userId}/{id}");
            Assert.AreEqual("/quotes/ian/1", uriFactory.CreateRelativeUri("ian", "1").ToString());
        }

        [Test]
        public void ShouldGenerateAbsoluteUriFromBaseAddressAndRoutePrefixAndTempleAndTemplateParameters()
        {
            var uriFactory = new UriFactoryWorker("quotes", "{userId}/{id}");
            Assert.AreEqual("http://restbucks.com/quotes/ian/1", uriFactory.CreateAbsoluteUri(new Uri("http://restbucks.com"), "ian", "1").ToString());
        }

        [Test]
        public void ShouldUseAllOfTheSuppliedBaseAddress()
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
        public void ShouldGenerateRelativeWithoutTerminatingBackslashWhenTemplateIsEmpty()
        {
            var uriFactory = new UriFactoryWorker("quotes");
            Assert.AreEqual("/quotes", uriFactory.CreateRelativeUri().ToString());
        }

        [Test]
        public void ShouldGenerateAbsoluteWithoutTerminatingBackslashWhenTemplateIsEmpty()
        {
            var uriFactory = new UriFactoryWorker("quotes");
            Assert.AreEqual("http://restbucks.com/quotes", uriFactory.CreateAbsoluteUri(new Uri("http://restbucks.com")).ToString());
        }

        [Test]
        public void ShouldGenerateBaseUriWithTerminatingBackslashFromSuppliedAbsoluteUri()
        {
            var uriFactory = new UriFactoryWorker("quotes", "{quoteId}");
            Assert.AreEqual(new Uri("http://restbucks.com:8080/uk/"), uriFactory.CreateBaseUri(new Uri("http://restbucks.com:8080/uk/quotes/1234")));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException), ExpectedMessage = "Supplied URI does not contain route prefix. Uri: [http://restbucks.com:8080/uk/customers/1234], Route prefix: [quotes].")]
        public void ThrowsExceptionWhenSuppliedUriDoesNotContainRoutePrefix()
        {
            var uriFactory = new UriFactoryWorker("quotes", "{quoteId}");
            uriFactory.CreateBaseUri(new Uri("http://restbucks.com:8080/uk/customers/1234"));
        }
    }
}