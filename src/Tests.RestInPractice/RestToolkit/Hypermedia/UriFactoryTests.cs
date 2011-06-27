using System;
using System.Collections.Generic;
using NUnit.Framework;
using RestInPractice.RestToolkit.Hypermedia;

namespace Tests.RestInPractice.RestToolkit.Hypermedia
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
            uriFactory.Register<OrderForm>();

            Assert.AreEqual(new Uri("http://localhost:8080/virtual-directory/"), uriFactory.CreateBaseUri<OrderForm>(new Uri("http://localhost:8080/virtual-directory/order-form/1")));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToCreateBaseUriForEntryWithoutRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.CreateBaseUri<OrderForm>(new Uri("http://localhost:8080/virtual-directory/order-form/1"));
        }

        [Test]
        public void ShouldCreateAbsoluteUriForRegisteredClass()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<OrderForm>();

            Assert.AreEqual(new Uri("http://localhost:8080/virtual-directory/order-form/1"), uriFactory.CreateAbsoluteUri<OrderForm>(new Uri("http://localhost:8080/virtual-directory/"), "1"));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToCreateAbsoluteUriForEntryWithoutRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.CreateAbsoluteUri<OrderForm>(new Uri("http://localhost:8080/virtual-directory/"), "1");
        }

        [Test]
        public void ShouldCreateRelativeUriForRegisteredClass()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<OrderForm>();

            Assert.AreEqual(new Uri("order-form/1", UriKind.Relative), uriFactory.CreateRelativeUri<OrderForm>("1"));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToCreateRelativeUriForEntryWithoutRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.CreateRelativeUri<OrderForm>("1");
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (ArgumentException))]
        public void ThrowsExceptionIfEntryAlreadyExistsForType()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<OrderForm>();
            uriFactory.Register<OrderForm>();
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
            uriFactory.Register<OrderForm>();

            Assert.AreEqual("order-form", uriFactory.GetRoutePrefix<OrderForm>());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToGetRoutePrefixForEntryWithoutRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.GetRoutePrefix<OrderForm>();
        }

        [Test]
        public void ShouldReturnUriTemplateValueForRegisteredClass()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<OrderForm>();

            Assert.AreEqual("{id}", uriFactory.GetUriTemplateValue<OrderForm>());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (KeyNotFoundException))]
        public void ThrowsExceptionIfTryingToGetUriTemplateValueForEntryWithoutRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.GetUriTemplateValue<OrderForm>();
        }

        [Test]
        public void WhenPassingGuidAsUriTemplateParameterShouldRemoveAllDashes()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<OrderForm>();

            Assert.AreEqual(new Uri("order-form/00000000000000000000000000000000", UriKind.Relative), uriFactory.CreateRelativeUri<OrderForm>(Guid.Empty));
        }

        [Test]
        public void ShouldReturnUriTemplateValueForRegisteredType()
        {
            var uriFactory = new UriFactory();
            uriFactory.Register<OrderForm>();

            Assert.AreEqual("{id}", uriFactory.GetUriTemplateValueFor(typeof (OrderForm)));
        }

        [Test]
        [ExpectedException(typeof (KeyNotFoundException))]
        public void ThrowsExceptionWhenTryingToGetUriTemplateValueForTypeThatHasNotBeenRegistered()
        {
            var uriFactory = new UriFactory();

            uriFactory.GetUriTemplateValueFor(typeof (OrderForm));
        }

        [UriTemplate("order-form", "{id}")]
        private class OrderForm
        {
        }

        [UriTemplate("quote", "{id}")]
        private class Quote
        {
        }
    }
}