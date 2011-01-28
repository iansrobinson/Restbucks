using System;
using NUnit.Framework;
using Restbucks.Quoting.Service.Old.Resources;

namespace Tests.Restbucks.Old.Quoting.Service.Old.Resources
{
    [TestFixture]
    public class UriFactoryTests
    {
        [Test]
        public void ShouldGenerateRelativeUriFromRoutePrefixAndTemplateAndTemplateParameters()
        {
            var uriFactory = new UriFactory("quotes", "{userId}/{id}");
            Assert.AreEqual("/quotes/ian/1", uriFactory.CreateRelativeUri("ian", "1").ToString());
        }

        [Test]
        public void ShouldGenerateAbsoluteUriFromBaseAddressAndRoutePrefixAndTempleAndTemplateParameters()
        {
            var uriFactory = new UriFactory("quotes", "{userId}/{id}");
            Assert.AreEqual("http://restbucks.com/quotes/ian/1", uriFactory.CreateAbsoluteUri(new Uri("http://restbucks.com"), "ian", "1").ToString());
        }

        [Test]
        public void ShouldUseOnlyTheHostAndPortPartOfTheSuppliedBaseAddress()
        {
            var uriFactory = new UriFactory("quotes", "{userId}/{id}");
            Assert.AreEqual("http://restbucks.com:8080/quotes/ian/1", uriFactory.CreateAbsoluteUri(new Uri("http://restbucks.com:8080/prefix/"), "ian", "1").ToString());
        }

        [Test]
        public void ShouldGenerateRelativeWithTerminatingBackslashWhenTemplateIsEmpty()
        {
            var uriFactory = new UriFactory("quotes");
            Assert.AreEqual("/quotes/", uriFactory.CreateRelativeUri().ToString());
        }

        [Test]
        public void ShouldGenerateAbosluteWithTerminatingBackslashWhenTemplateIsEmpty()
        {
            var uriFactory = new UriFactory("quotes");
            Assert.AreEqual("http://restbucks.com/quotes/", uriFactory.CreateAbsoluteUri(new Uri("http://restbucks.com")).ToString());
        }
    }
}