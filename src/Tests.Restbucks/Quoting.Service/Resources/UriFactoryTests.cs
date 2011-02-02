using System;
using System.Collections.Generic;
using NUnit.Framework;
using Restbucks.Quoting.Service.Resources;

namespace Tests.Restbucks.Quoting.Service.Resources
{
    [TestFixture]
    public class UriFactoryTests
    {
        [Test]
        public void ShouldReturnUriFactoryForClassAttributedWithUriTemplateAttribute()
        {
            var uriFactories = new UriFactory();
            uriFactories.Register<MyResource>();
            Assert.AreEqual("/my-resource/1", uriFactories.For<MyResource>().CreateRelativeUri("1").ToString());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException))]
        public void ThrowsExceptionIfEntryAlreadyExistsForType()
        {
            var uriFactories = new UriFactory();
            uriFactories.Register<MyResource>();
            uriFactories.Register<MyResource>();
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(KeyNotFoundException))]
        public void ThrowsExceptionIfEntryDoesNotExistForType()
        {
            var uriFactories = new UriFactory();
            uriFactories.For<MyResource>();
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(UriTemplateMissingException))]
        public void ThrowsExceptionIfTypeIsNOtAttributedWithUriTemplateAttribute()
        {
            var uriFactories = new UriFactory();
            uriFactories.Register<string>();
        }

        [UriTemplate("my-resource", "{id}")]
        private class MyResource
        {
        }
    }
}