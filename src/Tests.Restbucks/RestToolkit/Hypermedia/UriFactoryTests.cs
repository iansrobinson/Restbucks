using System;
using System.Collections.Generic;
using NUnit.Framework;
using Restbucks.Quoting.Service.Resources;
using Restbucks.RestToolkit.Hypermedia;

namespace Tests.Restbucks.RestToolkit.Hypermedia
{
    [TestFixture]
    public class UriFactoryTests
    {
        [Test]
        public void ShouldAllowRegistrationByPassingGenericParameterToRegisterMethod()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<Quote>();
            uriFactory.Register<OrderForm>();

            Assert.AreEqual(new Uri("http://restbucks.com/quote/1234"), uriFactory.CreateAbsoluteUri<Quote>(new Uri("http://restbucks.com"), 1234));
            Assert.AreEqual(new Uri("order-form/1234", UriKind.Relative), uriFactory.CreateRelativeUri<OrderForm>(1234));
            Assert.AreEqual(new Uri("http://restbucks.com/"), uriFactory.CreateBaseUri<Quote>(new Uri("http://restbucks.com/quote/1234")));
        }

        [Test]
        public void ShouldAllowRegistrationByPassingTypeToRegisterMethod()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register(typeof(Quote));
            uriFactory.Register(typeof(OrderForm));

            Assert.AreEqual(new Uri("http://restbucks.com/quote/1234"), uriFactory.CreateAbsoluteUri<Quote>(new Uri("http://restbucks.com"), 1234));
            Assert.AreEqual(new Uri("order-form/1234", UriKind.Relative), uriFactory.CreateRelativeUri<OrderForm>(1234));
            Assert.AreEqual(new Uri("http://restbucks.com/"), uriFactory.CreateBaseUri<Quote>(new Uri("http://restbucks.com/quote/1234")));
        }
        
        [Test]
        public void ShouldCreateBaseUriForRegisteredClass()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<MyResource>();

            Assert.AreEqual(new Uri("http://localhost:8080/virtual-directory/"), uriFactory.CreateBaseUri<MyResource>(new Uri("http://localhost:8080/virtual-directory/my-resource/1")));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToCreateBaseUriForEntryWithoutRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.CreateBaseUri<MyResource>(new Uri("http://localhost:8080/virtual-directory/my-resource/1"));
        }

        [Test]
        public void ShouldCreateAbsoluteUriForRegisteredClass()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<MyResource>();

            Assert.AreEqual(new Uri("http://localhost:8080/virtual-directory/my-resource/1"), uriFactory.CreateAbsoluteUri<MyResource>(new Uri("http://localhost:8080/virtual-directory/"), "1"));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToCreateAbsoluteUriForEntryWithoutRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.CreateAbsoluteUri<MyResource>(new Uri("http://localhost:8080/virtual-directory/"), "1");
        }

        [Test]
        public void ShouldCreateRelativeUriForRegisteredClass()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<MyResource>();

            Assert.AreEqual(new Uri("my-resource/1", UriKind.Relative), uriFactory.CreateRelativeUri<MyResource>("1"));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToCreateRelativeUriForEntryWithoutRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.CreateRelativeUri<MyResource>("1");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException))]
        public void ThrowsExceptionIfEntryAlreadyExistsForType()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<MyResource>();
            uriFactory.Register<MyResource>();
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (UriTemplateMissingException))]
        public void ThrowsExceptionIfTypeIsNotAttributedWithUriTemplateAttribute()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<string>();
        }

        [Test]
        public void ShouldReturnRoutePrefixForRegisteredClass()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<MyResource>();

            Assert.AreEqual("my-resource", uriFactory.GetRoutePrefix<MyResource>());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToGetRoutePrefixForEntryWithoutRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.GetRoutePrefix<MyResource>();
        }

        [Test]
        public void ShouldReturnUriTemplateValueForRegisteredClass()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<MyResource>();

            Assert.AreEqual("{id}", uriFactory.GetUriTemplateValue<MyResource>());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToGetUriTemplateValueForEntryWithoutRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.GetUriTemplateValue<MyResource>();
        }

        [Test]
        public void WhenPassingGuidAsUriTemplateParameterShouldRemoveAllDashes()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<MyResource>();

            Assert.AreEqual(new Uri("my-resource/00000000000000000000000000000000", UriKind.Relative), uriFactory.CreateRelativeUri<MyResource>(Guid.Empty));
        }

        [Test]
        public void ShouldReturnUriTemplateValueForRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<MyResource>();

            Assert.AreEqual("{id}", uriFactory.GetUriTemplateValueFor(typeof (MyResource)));
        }

        [Test]
        [ExpectedException(typeof (KeyNotFoundException))]
        public void ThrowsExceptionWhenTryingToGetUriTemplateValueForTypeThatHasNotBeenRegistered()
        {
            var uriFactory = new UriFactory();

            uriFactory.GetUriTemplateValueFor(typeof (MyResource));
        }

        [UriTemplate("my-resource", "{id}")]
        private class MyResource
        {
        }
    }
}