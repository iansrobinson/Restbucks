using System;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.MediaType.Assemblers;

namespace Tests.Restbucks.MediaType
{
    [TestFixture]
    public class LinkRelationEqualityComparerTests
    {
        [Test]
        public void CanBeUsedInLinqExpressions()
        {
            var shop = CreateShop();

            var links = (from l in shop.Links
                         where l.Rels.Contains(new StringLinkRelation("prefetch"), LinkRelationEqualityComparer.Instance)
                         select l);

            Assert.AreEqual(2, links.Count());
            Assert.AreEqual(new Uri("/quotes", UriKind.Relative), links.First().Href);
            Assert.AreEqual(new Uri("http://iansrobinson.com/help"), links.Last().Href);
        }

        [Test]
        public void ShouldBeCaseInsensitive()
        {
            var shop = CreateShop();

            var links = (from l in shop.Links
                         where l.Rels.Contains(new StringLinkRelation("PREFETCH"), LinkRelationEqualityComparer.Instance)
                         select l);

            Assert.AreEqual(2, links.Count());
            Assert.AreEqual(new Uri("/quotes", UriKind.Relative), links.First().Href);
            Assert.AreEqual(new Uri("http://iansrobinson.com/help"), links.Last().Href);
        }

        [Test]
        public void ShouldBeAbleToUseStringFormattedLinkRelationToFindUriLinkRelations()
        {
            var shop = CreateShop();

            var links = (from l in shop.Links
                         where l.Rels.Contains(new StringLinkRelation("http://relations.iansrobinson.com/help"), LinkRelationEqualityComparer.Instance)
                         select l);

            Assert.AreEqual(1, links.Count());
            Assert.AreEqual(new Uri("http://iansrobinson.com/help"), links.First().Href);
        }

        [Test]
        public void ShouldBeAbleToUseUriLinkRelationToFindUriLinkRelations()
        {
            var shop = CreateShop();

            var links = (from l in shop.Links
                         where l.Rels.Contains(new UriLinkRelation(new Uri("http://relations.iansrobinson.com/help")), LinkRelationEqualityComparer.Instance)
                         select l);

            Assert.AreEqual(1, links.Count());
            Assert.AreEqual(new Uri("http://iansrobinson.com/help"), links.First().Href);
        }

        [Test]
        public void ShouldBeAbleToUseCompactUriLinkRelationToFindUriLinkRelations()
        {
            var shop = CreateShop();

            var links = (from l in shop.Links
                         where l.Rels.Contains(new CompactUriLinkRelation("x", new Uri("http://relations.iansrobinson.com/"), "help"), LinkRelationEqualityComparer.Instance)
                         select l);

            Assert.AreEqual(1, links.Count());
            Assert.AreEqual(new Uri("http://iansrobinson.com/help"), links.First().Href);
        }

        private static Shop CreateShop()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns:rb=""http://relations.restbucks.com/"" xmlns=""http://schemas.restbucks.com/shop"">
  <link rel=""rb:rfq prefetch"" type=""application/xml"" href=""/quotes"" />
  <link rel=""rb:order-form"" type=""application/restbucks+xml"" href=""/order-forms/1234"" />
  <link rel=""prefetch http://relations.iansrobinson.com/help"" type=""text/html"" href=""http://iansrobinson.com/help"" />
</shop>";

            return new ShopAssembler(XElement.Parse(xml), new Uri("http://localhost/")).AssembleShop();
        }
    }
}