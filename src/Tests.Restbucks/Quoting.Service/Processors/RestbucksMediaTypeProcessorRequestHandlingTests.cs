using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Description;
using System.Text;
using Microsoft.ServiceModel.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.MediaType.Assemblers;
using Restbucks.Quoting.Service.Processors;

namespace Tests.Restbucks.Quoting.Service.Processors
{
    [TestFixture]
    public class RestbucksMediaTypeProcessorRequestHandlingTests
    {
        [Test]
        public void ShouldAssembleShopFromShopElement()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns=""http://schemas.restbucks.com/shop""/>";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                stream.Seek(0, SeekOrigin.Begin);

                var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

                var shop = mediaTypeProcessor.ReadFromStream(stream, new HttpRequestMessage()) as Shop;

                Assert.IsNotNull(shop);
                Assert.IsFalse(shop.HasItems);
                Assert.IsFalse(shop.HasLinks);
                Assert.IsFalse(shop.HasForms);
            }
        }

        [Test]
        public void ShouldAssembleShopWithLinksFromShopElementWithChildLinkElements()
        {
            const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<shop xmlns:rb=""http://relations.restbucks.com/"" xmlns=""http://schemas.restbucks.com/shop"">
  <link rel=""rb:rfq prefetch"" href=""/quotes"" />
  <link rel=""rb:order-form"" type=""application/restbucks+xml"" href=""/order-forms/1234"" />
</shop>";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                stream.Seek(0, SeekOrigin.Begin);

                var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

                var shop = mediaTypeProcessor.ReadFromStream(stream, new HttpRequestMessage()) as Shop;

                Assert.IsNotNull(shop);
                Assert.IsFalse(shop.HasItems);
                Assert.IsTrue(shop.HasLinks);
                Assert.IsFalse(shop.HasForms);

                Assert.AreEqual(2, shop.Links.Count());

                var firstLink = shop.Links.First();
                Assert.AreEqual("rb:rfq", firstLink.Rels.First().SerializableValue);
                Assert.AreEqual("prefetch", firstLink.Rels.Last().SerializableValue);
                Assert.AreEqual("/quotes", firstLink.Href.ToString());
                Assert.IsNull(firstLink.MediaType);

                var secondLink = shop.Links.Last();
                Assert.AreEqual("rb:order-form", secondLink.Rels.First().SerializableValue);
                Assert.AreEqual("/order-forms/1234", secondLink.Href.ToString());
                Assert.AreEqual("application/restbucks+xml", secondLink.MediaType);
            }
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

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                stream.Seek(0, SeekOrigin.Begin);

                var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

                var shop = mediaTypeProcessor.ReadFromStream(stream, new HttpRequestMessage()) as Shop;

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
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                stream.Seek(0, SeekOrigin.Begin);

                var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

                var shop = mediaTypeProcessor.ReadFromStream(stream, new HttpRequestMessage()) as Shop;

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
                Assert.AreEqual("application/restbucks+xml", firstForm.MediaType);

                var secondForm = shop.Forms.Last();
                Assert.IsNull(secondForm.Schema);
                Assert.IsNotNull(secondForm.Instance);
                Assert.AreEqual("/orders", secondForm.Resource.ToString());
                Assert.AreEqual("put", secondForm.Method);
                Assert.AreEqual("application/xml", secondForm.MediaType);
            }
        }

        [Test]
        public void ShouldReturnNullWhenStreamIsEmpty()
        {
            const string xml = @"";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

                Assert.IsNull(mediaTypeProcessor.ReadFromStream(stream, new HttpRequestMessage()));
            }
        }

        [Test]
        public void ShouldReturnNullWhenStreamIsNull()
        {
            var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();
            Assert.IsNull(mediaTypeProcessor.ReadFromStream(null, new HttpRequestMessage()));
        }

        [Test]
        public void ShouldReturnNullWhenStreamContainsInvalidXml()
        {
            const string xml = @"<invalid-xml><data/></invalid-xml>";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

                Assert.IsNull(mediaTypeProcessor.ReadFromStream(stream, new HttpRequestMessage()));
            }
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(InvalidFormatException), ExpectedMessage = "Incorrectly formatted entity body. Request must be formatted according to application/restbucks+xml.")]
        public void ShouldThrowExceptionWhenStreamContainsNonXmlData()
        {
            const string xml = @"non-xml-data";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

                mediaTypeProcessor.ReadFromStream(stream, new HttpRequestMessage());
            }
        }

        private static RestbucksMediaTypeProcessor CreateRestbucksMediaTypeProcessor()
        {
            return new RestbucksMediaTypeProcessor(new HttpOperationDescription(), MediaTypeProcessorMode.Response);
        }
    }
}