using System;
using NUnit.Framework;
using Restbucks.MediaType;

namespace Tests.Restbucks.MediaType
{
    [TestFixture]
    public class CompactUriLinkRelationTests
    {
        [Test]
        public void ValueShouldReturnAbsoluteUri()
        {
            var linkRelation = new CompactUriLinkRelation("rb", new Uri("http://relations.restbucks.com/"), "order-form");
            Assert.AreEqual("http://relations.restbucks.com/order-form", linkRelation.Value);
        }

        [Test]
        public void SerializableValueShouldReturnCompactUri()
        {
            var linkRelation = new CompactUriLinkRelation("rb", new Uri("http://relations.restbucks.com/"), "order-form");
            Assert.AreEqual("rb:order-form", linkRelation.SerializableValue);
        }

        [Test]
        public void ShouldIncludeFragmentIdentifierFromPrefixInValue()
        {
            var linkRelation = new CompactUriLinkRelation("rb", new Uri("http://relations.restbucks.com/#"), "order-form");
            Assert.AreEqual("http://relations.restbucks.com/#order-form", linkRelation.Value);
        }

        [Test]
        public void ShouldIncludeColonsFromReferenceInValue()
        {
            var linkRelation = new CompactUriLinkRelation("rb", new Uri("http://relations.restbucks.com/"), "order:first");
            Assert.AreEqual("http://relations.restbucks.com/order:first", linkRelation.Value);
        }
    }
}