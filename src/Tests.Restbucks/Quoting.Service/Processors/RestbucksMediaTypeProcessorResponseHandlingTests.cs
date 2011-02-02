using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Description;
using System.Xml;
using System.Xml.XPath;
using Microsoft.ServiceModel.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Quoting.Service;
using Restbucks.Quoting.Service.Processors;
using Tests.Restbucks.MediaType;

namespace Tests.Restbucks.Quoting.Service.Processors
{
    [TestFixture]
    public class RestbucksMediaTypeProcessorResponseHandlingTests
    {
        [Test]
        public void ShouldSupportRestbucksPlusXmlAndApplicationXmlAndTextXmlMediaTypes()
        {
            var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

            Assert.AreEqual(3, mediaTypeProcessor.SupportedMediaTypes.Count());
            Assert.AreEqual("application/restbucks+xml", mediaTypeProcessor.SupportedMediaTypes.First());
            Assert.AreEqual("application/xml", mediaTypeProcessor.SupportedMediaTypes.Skip(1).First());
            Assert.AreEqual("text/xml", mediaTypeProcessor.SupportedMediaTypes.Skip(2).First());
        }

        [Test]
        public void ShouldWriteShopRootElement()
        {
            var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

            using (Stream stream = new MemoryStream())
            {
                mediaTypeProcessor.WriteToStream(new ShopBuilder().Build(), stream, new HttpRequestMessage());

                Assert.AreEqual(1, new XmlOutput(stream).GetNodeCount("r:shop"));
            }
        }

        [Test]
        public void ShouldWriteLinksAsChildrenOfShopElement()
        {
            var shop = new ShopBuilder().Build()
                .AddLink(new Link(new Uri("/quotes", UriKind.Relative), LinkRelations.Rfq, LinkRelations.Prefetch))
                .AddLink(new Link("application/xml", new Uri("/order-forms/1234", UriKind.Relative), LinkRelations.OrderForm));

            var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

            using (Stream stream = new MemoryStream())
            {
                mediaTypeProcessor.WriteToStream(shop, stream, new HttpRequestMessage());

                var output = new XmlOutput(stream);

                Assert.AreEqual(2, output.GetNodeCount("r:shop/r:link"));

                Assert.AreEqual("rb:rfq prefetch", output.GetNodeValue("r:shop/r:link[1]/@rel"));
                Assert.AreEqual("/quotes", output.GetNodeValue("r:shop/r:link[1]/@href"));
                Assert.IsNull(output.GetNode("r:shop/r:link[1]/@type"));

                Assert.AreEqual("rb:order-form", output.GetNodeValue("r:shop/r:link[2]/@rel"));
                Assert.AreEqual("application/xml", output.GetNodeValue("r:shop/r:link[2]/@type"));
                Assert.AreEqual("/order-forms/1234", output.GetNodeValue("r:shop/r:link[2]/@href"));
            }
        }

        [Test]
        public void ShouldWriteCompactUriLinkRelationNamespacesToRootShopElement()
        {
            const string ns1 = "http://restbucks/relations/";
            const string ns2 = "http://thoughtworks/relations/";

            var rel1 = new CompactUriLinkRelation("rb", new Uri(ns1, UriKind.Absolute), "rel1");
            var rel2 = new CompactUriLinkRelation("tw", new Uri(ns2, UriKind.Absolute), "rel2");

            var shop = new ShopBuilder().Build()
                .AddLink(new Link(new Uri("/quotes", UriKind.Relative), rel1, rel2));

            var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

            using (Stream stream = new MemoryStream())
            {
                mediaTypeProcessor.WriteToStream(shop, stream, new HttpRequestMessage());

                var output = new XmlOutput(stream);
                
                Assert.AreEqual(ns1, output.GetNamespaceValue("rb"));
                Assert.AreEqual(ns2, output.GetNamespaceValue("tw"));
            }
        }

        [Test]
        public void ShouldWriteItemElementsAsChildrenOfItems()
        {
            var shop = new ShopBuilder().Build()
                .AddItem(new Item("item1", new Amount("g", 250)))
                .AddItem(new Item("item2", new Amount("g", 500), new Cost("GBP", 5.50)));

            var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

            using (Stream stream = new MemoryStream())
            {
                mediaTypeProcessor.WriteToStream(shop, stream, new HttpRequestMessage());

                var output = new XmlOutput(stream);

                Assert.AreEqual(2, output.GetNodeCount("r:shop/r:items/r:item"));

                Assert.AreEqual("item1", output.GetNodeValue("r:shop/r:items/r:item[1]/r:description"));
                Assert.AreEqual("g", output.GetNodeValue("r:shop/r:items/r:item[1]/r:amount/@measure"));
                Assert.AreEqual("250", output.GetNodeValue("r:shop/r:items/r:item[1]/r:amount"));
                Assert.IsNull(output.GetNode("r:shop/r:items/r:item[1]/r:price"));

                Assert.AreEqual("item2", output.GetNodeValue("r:shop/r:items/r:item[2]/r:description"));
                Assert.AreEqual("g", output.GetNodeValue("r:shop/r:items/r:item[2]/r:amount/@measure"));
                Assert.AreEqual("500", output.GetNodeValue("r:shop/r:items/r:item[2]/r:amount"));
                Assert.AreEqual("GBP", output.GetNodeValue("r:shop/r:items/r:item[2]/r:price/@currency"));
                Assert.AreEqual("5.50", output.GetNodeValue("r:shop/r:items/r:item[2]/r:price"));
            }
        }

        [Test]
        public void ShouldNotOutputItemsElementIfThereAreNoItemsInShop()
        {
            var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

            using (Stream stream = new MemoryStream())
            {
                mediaTypeProcessor.WriteToStream(new ShopBuilder().Build(), stream, new HttpRequestMessage());

                Assert.IsNull(new XmlOutput(stream).GetNode("r:shop/r:items"));
            }
        }

        [Test]
        public void ShouldWriteFormsAsChildrenOfShopElement()
        {
            var shop = new ShopBuilder().Build()
                .AddForm(new Form(new Uri("/quotes", UriKind.Relative), "post", "application/restbucks+xml", new Uri("http://schemas.restbucks.com/shop.xsd")))
                .AddForm(new Form(new Uri("/orders", UriKind.Relative), "put", "application/restbucks+xml", new ShopBuilder().Build()));

            var mediaTypeProcessor = CreateRestbucksMediaTypeProcessor();

            using (Stream stream = new MemoryStream())
            {
                mediaTypeProcessor.WriteToStream(shop, stream, new HttpRequestMessage());

                var output = new XmlOutput(stream);

                Assert.AreEqual(2, output.GetNodeCount("r:shop/x:model"));

                Assert.AreEqual("http://schemas.restbucks.com/shop.xsd", output.GetNodeValue("r:shop/x:model[1]/@schema"));
                Assert.AreEqual("/quotes", output.GetNodeValue("r:shop/x:model[1]/x:submission/@resource"));
                Assert.AreEqual("post", output.GetNodeValue("r:shop/x:model[1]/x:submission/@method"));
                Assert.AreEqual("application/restbucks+xml", output.GetNodeValue("r:shop/x:model[1]/x:submission/@mediatype"));
                Assert.AreEqual(string.Empty, output.GetNodeValue("r:shop/x:model[1]/x:instance"));

                Assert.IsNull(output.GetNode("r:shop/x:model[2]/@schema"));
                Assert.AreEqual("/orders", output.GetNodeValue("r:shop/x:model[2]/x:submission/@resource"));
                Assert.AreEqual("put", output.GetNodeValue("r:shop/x:model[2]/x:submission/@method"));
                Assert.AreEqual("application/restbucks+xml", output.GetNodeValue("r:shop/x:model[2]/x:submission/@mediatype"));
                Assert.AreEqual(string.Empty, output.GetNodeValue("r:shop/x:model[2]/x:instance/r:shop"));
            }
        }

        private static RestbucksMediaTypeProcessor CreateRestbucksMediaTypeProcessor()
        {
            return new RestbucksMediaTypeProcessor(new HttpOperationDescription(), MediaTypeProcessorMode.Response);
        }

        private static void PrintStream(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(stream);
            Console.WriteLine(reader.ReadToEnd());
        }

        private class XmlOutput
        {
            private readonly XPathNavigator navigator;
            private readonly XmlNamespaceManager manager;

            public XmlOutput(Stream stream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                var document = new XPathDocument(stream);
                navigator = document.CreateNavigator();
                manager = new XmlNamespaceManager(navigator.NameTable);
                manager.AddNamespace("r", Namespaces.ShopSchema.NamespaceName);
                manager.AddNamespace("x", Namespaces.XForms.NamespaceName);
            }

            public int GetNodeCount(string xpath)
            {
                return navigator.Select(xpath, manager).Count;
            }

            public string GetNodeValue(string xpath)
            {
                return navigator.SelectSingleNode(xpath, manager).Value;
            }

            public string GetNamespaceValue(string prefix)
            {
                return navigator.SelectSingleNode("*/.", manager).GetNamespacesInScope(XmlNamespaceScope.All)[prefix];
            }

            public XPathNavigator GetNode(string xpath)
            {
                return navigator.SelectSingleNode(xpath, manager);
            }
        }
    }
}