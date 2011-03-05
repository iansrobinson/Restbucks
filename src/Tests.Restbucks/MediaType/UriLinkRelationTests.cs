using System;
using NUnit.Framework;
using Restbucks.MediaType;

namespace Tests.Restbucks.MediaType
{
    [TestFixture]
    public class UriLinkRelationTests
    {
        [Test]
        public void ValueShouldReturnAbsoluteUri()
        {
            var linkRelation = new UriLinkRelation(new Uri("http://relations.restbucks.com/order-form"));
            Assert.AreEqual("http://relations.restbucks.com/order-form", linkRelation.Value);
        }

        [Test]
        public void SerializableValueShouldReturnAbsoluteUri()
        {
            var linkRelation = new UriLinkRelation(new Uri("http://relations.restbucks.com/order-form"));
            Assert.AreEqual("http://relations.restbucks.com/order-form", linkRelation.DisplayValue);
        }
    }
}