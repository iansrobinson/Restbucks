using System;
using NUnit.Framework;
using Restbucks.MediaType;

namespace Tests.Restbucks.MediaType
{
    [TestFixture]
    public class ShopTests
    {
        private static readonly Uri Ns1 = new Uri("http://restbucks/relations/");
        private static readonly Uri Ns2 = new Uri("http://thoughtworks/relations/");

        private static readonly LinkRelation RbNs1 = new CompactUriLinkRelation("rb", Ns1, "rel1");
        private static readonly LinkRelation TwNs1 = new CompactUriLinkRelation("tw", Ns1, "rel2");
        private static readonly LinkRelation RbNs2 = new CompactUriLinkRelation("rb", Ns2, "rel3");
        private static readonly LinkRelation TwNs2 = new CompactUriLinkRelation("tw", Ns2, "rel4");

        [Test]
        [ExpectedException(ExpectedException = typeof (NamespacePrefixConflictException), ExpectedMessage = "One or more namespace prefixes are each associated with more than one namespace: 'rb'.")]
        public void ShouldThrowExceptionWhenAddingLinkWithACompactUriLinkRelationWithSamePrefixAsExistingLinkRelationButDifferentNamespace()
        {
            new ShopBuilder(new Uri("http://localhost"))
                .AddLink(new Link(new Uri("http://localhost/link1"), RestbucksMediaType.Value, RbNs1))
                .AddLink(new Link(new Uri("http://localhost/link2"), RestbucksMediaType.Value, RbNs2))
                .Build();
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (NamespacePrefixConflictException), ExpectedMessage = "One or more namespace prefixes are each associated with more than one namespace: 'rb'.")]
        public void ShouldThrowExceptionWhenAddingLinkWithCompactUriLinkRelationsWithSamePrefixButDifferentNamespace()
        {
            new ShopBuilder(new Uri("http://localhost"))
                .AddLink(new Link(new Uri("http://localhost/link1"), RestbucksMediaType.Value, RbNs1, RbNs2))
                .Build();
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (NamespacePrefixConflictException), ExpectedMessage = "One or more namespace prefixes are each associated with more than one namespace: 'rb, tw'.")]
        public void ShouldThrowExceptionWhenAddingLinkWithMoreThanOneCompactUriLinkRelationWithSamePrefixAsExistingLinkRelationButDifferentNamespace()
        {
            new ShopBuilder(new Uri("http://localhost"))
                .AddLink(new Link(new Uri("http://localhost/link1"), RestbucksMediaType.Value, RbNs1, TwNs1))
                .AddLink(new Link(new Uri("http://localhost/link2"), RestbucksMediaType.Value, RbNs2, TwNs2))
                .Build();
        }

        [Test]
        public void DoesNotThrowExceptionWhenAddingMoreThanOneLinkWithCompactUriLnkRelationsWithSamePrefixes()
        {
            new ShopBuilder(new Uri("http://localhost"))
                .AddLink(new Link(new Uri("http://localhost/link1"), RestbucksMediaType.Value, RbNs1, TwNs1, RbNs1))
                .AddLink(new Link(new Uri("http://localhost/link1"), RestbucksMediaType.Value, RbNs1, TwNs1, RbNs1))
                .Build();
        }

        [Test]
        public void ShouldUseSuppliedUriAsBaseUri()
        {
            var shop = new ShopBuilder(new Uri("http://localhost:8080/virtual-directory")).Build();
            Assert.AreEqual(new Uri("http://localhost:8080/virtual-directory"), shop.BaseUri);
        }
    }
}