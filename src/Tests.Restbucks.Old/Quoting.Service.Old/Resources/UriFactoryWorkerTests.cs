using System;
using NUnit.Framework;
using Restbucks.Quoting.Service.Old.Resources;

namespace Tests.Restbucks.Old.Quoting.Service.Old.Resources
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
        public void ShouldUseAllOfTheSuppliedBaseAddressUpToLastBackslash()
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
        public void ShouldGenerateRelativeUriWithTerminatingBackslashWhenTemplateIsBackslash()
        {
            var uriFactory = new UriFactoryWorker("quotes", "/");
            Assert.AreEqual("/quotes/", uriFactory.CreateRelativeUri().ToString());
        }

        [Test]
        public void ShouldGenerateAbsoluteUriWithoutTerminatingBackslashWhenTemplateIsEmpty()
        {
            var uriFactory = new UriFactoryWorker("quotes");
            Assert.AreEqual("http://restbucks.com/quotes", uriFactory.CreateAbsoluteUri(new Uri("http://restbucks.com")).ToString());
        }

        [Test]
        public void ShouldGenerateAbsoluteUriWithTerminatingBackslashWhenTemplateIsBackslash()
        {
            var uriFactory = new UriFactoryWorker("quotes", "/");
            Assert.AreEqual("http://restbucks.com/quotes/", uriFactory.CreateAbsoluteUri(new Uri("http://restbucks.com")).ToString());
        }
    }
}