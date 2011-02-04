using System;
using System.Collections.Generic;
using NUnit.Framework;
using Restbucks.RestToolkit;

namespace Tests.Restbucks.RestToolkit
{
    [TestFixture]
    public class UriFactoryTests
    {
        [Test]
        public void ShouldCreateBaseUriForRegisteredClass()
        {
            var uriFactories = new UriFactory();
            uriFactories.Register<MyResource>();

            Assert.AreEqual(new Uri("http://localhost:8080/virtual-directory/"), uriFactories.CreateBaseUri<MyResource>(new Uri("http://localhost:8080/virtual-directory/my-resource/1")));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
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
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
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
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToCreateRelativeUriForEntryWithoutRegisteredType()
        {
            var uriFactories = new UriFactory();
            uriFactories.CreateRelativeUri<MyResource>("1");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException))]
        public void ThrowsExceptionIfEntryAlreadyExistsForType()
        {
            var uriFactories = new UriFactory();
            uriFactories.Register<MyResource>();
            uriFactories.Register<MyResource>();
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (UriTemplateMissingException))]
        public void ThrowsExceptionIfTypeIsNotAttributedWithUriTemplateAttribute()
        {
            var uriFactories = new UriFactory();
            uriFactories.Register<string>();
        }

        [Test]
        public void ShouldReturnRoutePrefixForRegisteredClass()
        {
            var uriFactories = new UriFactory();
            uriFactories.Register<MyResource>();

            Assert.AreEqual("my-resource", uriFactories.GetRoutePrefix<MyResource>());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToGetRoutePrefixForEntryWithoutRegisteredType()
        {
            var uriFactories = new UriFactory();
            uriFactories.GetRoutePrefix<MyResource>();
        }

        [Test]
        public void ShouldReturnUriTemplateValueForRegisteredClass()
        {
            var uriFactories = new UriFactory();
            uriFactories.Register<MyResource>();

            Assert.AreEqual("{id}", uriFactories.GetUriTemplateValue<MyResource>());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToGetUriTemplateValueForEntryWithoutRegisteredType()
        {
            var uriFactories = new UriFactory();
            uriFactories.GetUriTemplateValue<MyResource>();
        }

        [Test]
        public void WhenPassingGuidAsUriTemplateParameterShouldRemoveAllDashes()
        {
            var uriFactories = new UriFactory();
            uriFactories.Register<MyResource>();

            Assert.AreEqual(new Uri("/my-resource/00000000000000000000000000000000", UriKind.Relative), uriFactories.CreateRelativeUri<MyResource>(Guid.Empty));
        }

        [Test]
        public void ShouldReturnUriTemplateValueForRegisteredType()
        {
            var uriFactories = new UriFactory();
            uriFactories.Register<MyResource>();

            Assert.AreEqual("{id}", uriFactories.GetUriTemplateValueFor(typeof(MyResource)));
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void ThrowsExceptionWhenTryingToGetUriTemplateValueForTypeThatHasNotBeenRegistered()
        {
            var uriFactories = new UriFactory();
            
            uriFactories.GetUriTemplateValueFor(typeof(MyResource));
        }

        [UriTemplate("my-resource", "{id}")]
        private class MyResource
        {
        }
    }
}