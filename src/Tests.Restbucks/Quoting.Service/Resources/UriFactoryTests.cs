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
        public void ShouldCreateBaseUriForRegisteredClass()
        {
            var uriFactories = new UriFactory();
            uriFactories.Register<MyResource>();

            Assert.AreEqual(new Uri("http://localhost:8080/virtual-directory/"), uriFactories.CreateBaseUri<MyResource>(new Uri("http://localhost:8080/virtual-directory/my-resource/1")));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToCreateBaseUriForEntryWithoutRegisteredType()
        {
            var uriFactories = new UriFactory();
            uriFactories.CreateBaseUri<MyResource>(new Uri("http://localhost:8080/virtual-directory/my-resource/1"));
        }

        [Test]
        public void ShouldCreateAbsoluteUriForRegisteredClass()
        {
            var uriFactories = new UriFactory();
            uriFactories.Register<MyResource>();

            Assert.AreEqual(new Uri("http://localhost:8080/virtual-directory/my-resource/1"), uriFactories.CreateAbsoluteUri<MyResource>(new Uri("http://localhost:8080/virtual-directory/"), "1"));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToCreateAbsoluteUriForEntryWithoutRegisteredType()
        {
            var uriFactories = new UriFactory();
            uriFactories.CreateAbsoluteUri<MyResource>(new Uri("http://localhost:8080/virtual-directory/"), "1");
        }

        [Test]
        public void ShouldCreateRelativeUriForRegisteredClass()
        {
            var uriFactories = new UriFactory();
            uriFactories.Register<MyResource>();

            Assert.AreEqual(new Uri("/my-resource/1", UriKind.Relative), uriFactories.CreateRelativeUri<MyResource>("1"));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToCreateRelativeUriForEntryWithoutRegisteredType()
        {
            var uriFactories = new UriFactory();
            uriFactories.CreateRelativeUri<MyResource>("1");
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
        public void ThrowsExceptionIfTypeIsNotAttributedWithUriTemplateAttribute()
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