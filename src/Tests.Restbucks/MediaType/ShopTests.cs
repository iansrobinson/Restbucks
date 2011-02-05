using System;
using System.Linq;
using NUnit.Framework;
using Restbucks.MediaType;
using Tests.Restbucks.MediaType.Helpers;

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
            new ShopBuilder().Build()
                .AddLink(new Link(new Uri("http://localhost/link1"), RestbucksMediaType.Value, RbNs1))
                .AddLink(new Link(new Uri("http://localhost/link2"), RestbucksMediaType.Value, RbNs2));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(NamespacePrefixConflictException), ExpectedMessage = "One or more namespace prefixes are each associated with more than one namespace: 'rb'.")]
        public void ShouldThrowExceptionWhenAddingLinkWithCompactUriLinkRelationsWithSamePrefixButDifferentNamespace()
        {
            new ShopBuilder().Build()
                .AddLink(new Link(new Uri("http://localhost/link1"), RestbucksMediaType.Value, RbNs1, RbNs2));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(NamespacePrefixConflictException), ExpectedMessage = "One or more namespace prefixes are each associated with more than one namespace: 'rb, tw'.")]
        public void ShouldThrowExceptionWhenAddingLinkWithMoreThanOneCompactUriLinkRelationWithSamePrefixAsExistingLinkRelationButDifferentNamespace()
        {
            new ShopBuilder().Build()
                .AddLink(new Link(new Uri("http://localhost/link1"), RestbucksMediaType.Value, RbNs1, TwNs1))
                .AddLink(new Link(new Uri("http://localhost/link2"), RestbucksMediaType.Value, RbNs2, TwNs2));
        }

        [Test]
        public void ShouldUseSuppliedUriAsBaseUri()
        {
            var shop = new ShopBuilder().WithBaseUri(new Uri("http://localhost:8080/virtual-directory")).Build();
            Assert.AreEqual(new Uri("http://localhost:8080/virtual-directory"), shop.BaseUri);
        }

        [Test]
        public void WhenAddingLinksShouldAddBaseUriToLinksWithRelativeHrefs()
        {
            var shop = new ShopBuilder().WithBaseUri(new Uri("http://localhost:8080/virtual-directory")).Build();
            shop.AddLink(new Link(new Uri("/quotes", UriKind.Relative), RestbucksMediaType.Value));

            Uri clickUri = null;
            shop.Links.First().Click((uri, prevResponse) =>
                                         {
                                             clickUri = uri;
                                             return null;
                                         });

            Assert.AreEqual(new Uri("http://localhost:8080/virtual-directory/quotes"), clickUri);
            Assert.AreEqual(new Uri("/quotes", UriKind.Relative), shop.Links.First().Href);
        }
    }
}