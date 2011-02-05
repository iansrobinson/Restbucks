using System;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.MediaType.Assemblers;

namespace Tests.Restbucks.MediaType.Assemblers
{
    [TestFixture]
    public class ShopAssemblerTests
    {
        [Test]
        public void ShouldAssembleShopFromShopElement()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop""/>";

            var shop = new ShopAssembler(XElement.Parse(xml), new Uri("http://localhost/")).AssembleShop();

            Assert.IsNotNull(shop);
            Assert.IsFalse(shop.HasItems);
            Assert.IsFalse(shop.HasLinks);
            Assert.IsFalse(shop.HasForms);
        }

        [Test]
        public void ShouldAssembleShopWithLinksFromShopElementWithChildLinkElements()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns:rb=""http://relations.restbucks.com/"" xmlns=""http://schemas.restbucks.com/shop"">
  <link rel=""rb:rfq prefetch"" type=""application/xml"" href=""/quotes"" />
  <link rel=""rb:order-form"" type=""application/restbucks+xml"" href=""/order-forms/1234"" />
</shop>";

            var shop = new ShopAssembler(XElement.Parse(xml), new Uri("http://localhost/")).AssembleShop();

            Assert.IsNotNull(shop);
            Assert.IsFalse(shop.HasItems);
            Assert.IsTrue(shop.HasLinks);
            Assert.IsFalse(shop.HasForms);

            Assert.AreEqual(2, shop.Links.Count());

            var firstLink = shop.Links.First();
            Assert.AreEqual("rb:rfq", firstLink.Rels.First().SerializableValue);
            Assert.AreEqual("prefetch", firstLink.Rels.Last().SerializableValue);
            Assert.AreEqual("/quotes", firstLink.Href.ToString());
            Assert.AreEqual("application/xml", firstLink.MediaType);

            var secondLink = shop.Links.Last();
            Assert.AreEqual("rb:order-form", secondLink.Rels.First().SerializableValue);
            Assert.AreEqual("/order-forms/1234", secondLink.Href.ToString());
            Assert.AreEqual(RestbucksMediaType.Value, secondLink.MediaType);
        }

        [Test]
        public void ShouldAssembleShopWithItemsFromShopElementWithItemsElements()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <items>
    <item>
      <description>item1</description>
      <amount measure=""g"">250</amount>
    </item>
    <item>
      <description>item2</description>
      <amount measure=""g"">500</amount>
      <price currency=""GBP"">5.50</price>
    </item>
  </items>
</shop>";

            var shop = new ShopAssembler(XElement.Parse(xml), new Uri("http://localhost/")).AssembleShop();

            Assert.IsNotNull(shop);
            Assert.IsTrue(shop.HasItems);
            Assert.IsFalse(shop.HasLinks);
            Assert.IsFalse(shop.HasForms);

            Assert.AreEqual(2, shop.Items.Count());

            var firstItem = shop.Items.First();
            Assert.AreEqual("g", firstItem.Amount.Measure);
            Assert.AreEqual(250, firstItem.Amount.Value);
            Assert.IsNull(firstItem.Cost);
            Assert.AreEqual("item1", firstItem.Description);

            var secondItem = shop.Items.Last();
            Assert.AreEqual("g", secondItem.Amount.Measure);
            Assert.AreEqual(500, secondItem.Amount.Value);
            Assert.AreEqual("GBP", secondItem.Cost.Currency);
            Assert.AreEqual(5.50, secondItem.Cost.Value);
            Assert.AreEqual("item2", secondItem.Description);
        }

        [Test]
        public void ShouldAssembleShopWithFormsFromShopElementWithChildFormElements()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <model schema=""http://schemas.restbucks.com/shop.xsd"" xmlns=""http://www.w3.org/2002/xforms"">
    <instance />
    <submission resource=""/quotes"" method=""post"" mediatype=""application/restbucks+xml"" />
  </model>
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"" />
    </instance>
    <submission resource=""/orders"" method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";
            var shop = new ShopAssembler(XElement.Parse(xml), new Uri("http://localhost/")).AssembleShop();

            Assert.IsNotNull(shop);
            Assert.IsFalse(shop.HasItems);
            Assert.IsFalse(shop.HasLinks);
            Assert.IsTrue(shop.HasForms);

            Assert.AreEqual(2, shop.Forms.Count());

            var firstForm = shop.Forms.First();
            Assert.AreEqual("http://schemas.restbucks.com/shop.xsd", firstForm.Schema.ToString());
            Assert.IsNull(firstForm.Instance);
            Assert.AreEqual("/quotes", firstForm.Resource.ToString());
            Assert.AreEqual("post", firstForm.Method);
            Assert.AreEqual(RestbucksMediaType.Value, firstForm.MediaType);

            var secondForm = shop.Forms.Last();
            Assert.IsNull(secondForm.Schema);
            Assert.IsNotNull(secondForm.Instance);
            Assert.AreEqual("/orders", secondForm.Resource.ToString());
            Assert.AreEqual("put", secondForm.Method);
            Assert.AreEqual("application/xml", secondForm.MediaType);
        }

        [Test]
        public void IfRootDoesNotSpecifyXmlBaseFallsBackToParentBaseUri()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop""/>";

            var shop = new ShopAssembler(XElement.Parse(xml), new Uri("http://restbucks.com:8080/shop")).AssembleShop();

            Assert.AreEqual(new Uri("http://restbucks.com:8080/shop"), shop.BaseUri);
        }

        [Test]
        public void ShouldUseXmlBaseValueFromRootIfPresent()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"" xml:base=""http://iansrobinson.com/""/>";

            var shop = new ShopAssembler(XElement.Parse(xml), new Uri("http://restbucks.com:8080/shop")).AssembleShop();

            Assert.AreEqual(new Uri("http://iansrobinson.com/"), shop.BaseUri);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof (BaseUriMissingException))]
        public void ShouldThrowExceptionIfOneOrMoreLinksContainRelativeUriButNoBaseUriIsAvailable()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns:rb=""http://relations.restbucks.com/"" xmlns=""http://schemas.restbucks.com/shop"">
  <link rel=""rb:rfq prefetch"" type=""application/xml"" href=""http://localhost/quotes"" />
  <link rel=""rb:order-form"" type=""application/restbucks+xml"" href=""/order-forms/1234"" />
</shop>";

            new ShopAssembler(XElement.Parse(xml), null).AssembleShop();
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(InvalidFormatException), ExpectedMessage = "Invalid format. Base URI missing.")]
        public void ShouldThrowExceptionIfOneOrMoreFormsContainRelativeUriButNoBaseUriIsAvailable()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"">
  <model schema=""http://schemas.restbucks.com/shop.xsd"" xmlns=""http://www.w3.org/2002/xforms"">
    <instance />
    <submission resource=""http://localhost/quotes"" method=""post"" mediatype=""application/restbucks+xml"" />
  </model>
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"" />
    </instance>
    <submission resource=""/orders"" method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";

            new ShopAssembler(XElement.Parse(xml), null).AssembleShop();
        }

        [Test]
        public void ShouldPassBaseUriToFormContents()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"" xml:base=""http://restbucks.com/"">
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop""/>
    </instance>
    <submission resource=""http://localhost/orders"" method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";
            var shop = new ShopAssembler(XElement.Parse(xml), null).AssembleShop();

            Assert.AreEqual(new Uri("http://restbucks.com"), shop.Forms.First().Instance.BaseUri);
        }

        [Test]
        public void FormContentsShouldUseOwnBaseUriIfPresent()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop"" xml:base=""http://restbucks.com/"">
  <model xmlns=""http://www.w3.org/2002/xforms"">
    <instance>
      <shop xmlns=""http://schemas.restbucks.com/shop"" xml:base=""http://iansrobinson.com/""/>
    </instance>
    <submission resource=""http://localhost/orders"" method=""put"" mediatype=""application/xml"" />
  </model>
</shop>";
            var shop = new ShopAssembler(XElement.Parse(xml), null).AssembleShop();

            Assert.AreEqual(new Uri("http://iansrobinson.com"), shop.Forms.First().Instance.BaseUri);
        }
    }
}