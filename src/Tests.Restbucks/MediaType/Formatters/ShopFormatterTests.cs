using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.MediaType.Formatters;
using Restbucks.Quoting.Service;
using Tests.Restbucks.MediaType.Helpers;

namespace Tests.Restbucks.MediaType.Formatters
{
    [TestFixture]
    public class ShopFormatterTests
    {
        [Test]
        public void ShouldCreateShopRootElement()
        {
            var formatter = new ShopFormatter(new ShopBuilder().Build());
            var xml = new XmlOutput(formatter.CreateXml());

            Assert.AreEqual(1, xml.GetNodeCount("r:shop"));
        }

        [Test]
        public void ShouldCreateLinksAsChildrenOfShopElement()
        {
            var shop = new ShopBuilder().Build()
                .AddLink(new Link(new Uri("/quotes", UriKind.Relative), RestbucksMediaType.Value, LinkRelations.Rfq, LinkRelations.Prefetch))
                .AddLink(new Link(new Uri("/order-forms/1234", UriKind.Relative), "application/xml", LinkRelations.OrderForm));

            var xml = new XmlOutput(new ShopFormatter(shop).CreateXml());

            Assert.AreEqual(2, xml.GetNodeCount("r:shop/r:link"));

            Assert.AreEqual("rb:rfq prefetch", xml.GetNodeValue("r:shop/r:link[1]/@rel"));
            Assert.AreEqual("/quotes", xml.GetNodeValue("r:shop/r:link[1]/@href"));
            Assert.AreEqual(RestbucksMediaType.Value, xml.GetNodeValue("r:shop/r:link[1]/@type"));

            Assert.AreEqual("rb:order-form", xml.GetNodeValue("r:shop/r:link[2]/@rel"));
            Assert.AreEqual("application/xml", xml.GetNodeValue("r:shop/r:link[2]/@type"));
            Assert.AreEqual("/order-forms/1234", xml.GetNodeValue("r:shop/r:link[2]/@href"));
        }

        [Test]
        public void ShouldAddCompactUriLinkRelationNamespacesToRootShopElement()
        {
            const string ns1 = "http://restbucks/relations/";
            const string ns2 = "http://thoughtworks/relations/";

            var rel1 = new CompactUriLinkRelation("rb", new Uri(ns1, UriKind.Absolute), "rel1");
            var rel2 = new CompactUriLinkRelation("tw", new Uri(ns2, UriKind.Absolute), "rel2");

            var shop = new ShopBuilder().Build()
                .AddLink(new Link(new Uri("/quotes", UriKind.Relative), RestbucksMediaType.Value, rel1, rel2));

            var xml = new XmlOutput(new ShopFormatter(shop).CreateXml());

            Assert.AreEqual(ns1, xml.GetNamespaceValue("rb"));
            Assert.AreEqual(ns2, xml.GetNamespaceValue("tw"));
        }

        [Test]
        public void ShouldAddItemElementsAsChildrenOfItems()
        {
            var shop = new ShopBuilder().Build()
                .AddItem(new Item("item1", new Amount("g", 250)))
                .AddItem(new Item("item2", new Amount("g", 500), new Cost("GBP", 5.50)));

            var xml = new XmlOutput(new ShopFormatter(shop).CreateXml());

            Assert.AreEqual(2, xml.GetNodeCount("r:shop/r:items/r:item"));

            Assert.AreEqual("item1", xml.GetNodeValue("r:shop/r:items/r:item[1]/r:description"));
            Assert.AreEqual("g", xml.GetNodeValue("r:shop/r:items/r:item[1]/r:amount/@measure"));
            Assert.AreEqual("250", xml.GetNodeValue("r:shop/r:items/r:item[1]/r:amount"));
            Assert.IsNull(xml.GetNode("r:shop/r:items/r:item[1]/r:price"));

            Assert.AreEqual("item2", xml.GetNodeValue("r:shop/r:items/r:item[2]/r:description"));
            Assert.AreEqual("g", xml.GetNodeValue("r:shop/r:items/r:item[2]/r:amount/@measure"));
            Assert.AreEqual("500", xml.GetNodeValue("r:shop/r:items/r:item[2]/r:amount"));
            Assert.AreEqual("GBP", xml.GetNodeValue("r:shop/r:items/r:item[2]/r:price/@currency"));
            Assert.AreEqual("5.50", xml.GetNodeValue("r:shop/r:items/r:item[2]/r:price"));
        }

        [Test]
        public void ShouldNotAddItemsElementIfThereAreNoItemsInShop()
        {
            var shop = new ShopBuilder().Build();
            var xml = new XmlOutput(new ShopFormatter(shop).CreateXml());
            Assert.IsNull(xml.GetNode("r:shop/r:items"));
        }

        [Test]
        public void ShouldAddFormsAsChildrenOfShopElement()
        {
            var shop = new ShopBuilder().Build()
                .AddForm(new Form(new Uri("/quotes", UriKind.Relative), "post", RestbucksMediaType.Value, new Uri("http://schemas.restbucks.com/shop.xsd")))
                .AddForm(new Form(new Uri("/orders", UriKind.Relative), "put", RestbucksMediaType.Value, new ShopBuilder().Build()));

            var output = new XmlOutput(new ShopFormatter(shop).CreateXml());

            Assert.AreEqual(2, output.GetNodeCount("r:shop/x:model"));

            Assert.AreEqual("http://schemas.restbucks.com/shop.xsd", output.GetNodeValue("r:shop/x:model[1]/@schema"));
            Assert.AreEqual("/quotes", output.GetNodeValue("r:shop/x:model[1]/x:submission/@resource"));
            Assert.AreEqual("post", output.GetNodeValue("r:shop/x:model[1]/x:submission/@method"));
            Assert.AreEqual(RestbucksMediaType.Value, output.GetNodeValue("r:shop/x:model[1]/x:submission/@mediatype"));
            Assert.AreEqual(string.Empty, output.GetNodeValue("r:shop/x:model[1]/x:instance"));

            Assert.IsNull(output.GetNode("r:shop/x:model[2]/@schema"));
            Assert.AreEqual("/orders", output.GetNodeValue("r:shop/x:model[2]/x:submission/@resource"));
            Assert.AreEqual("put", output.GetNodeValue("r:shop/x:model[2]/x:submission/@method"));
            Assert.AreEqual(RestbucksMediaType.Value, output.GetNodeValue("r:shop/x:model[2]/x:submission/@mediatype"));
            Assert.AreEqual(string.Empty, output.GetNodeValue("r:shop/x:model[2]/x:instance/r:shop"));
        }

        [Test] 
        public void ShouldAddXmlBaseAttributeToRootElement()
        {
            var formatter = new ShopFormatter(new ShopBuilder().WithBaseUri(new Uri("http://restbucks.com:8080/shop")).Build());
            var xml = new XmlOutput(formatter.CreateXml());

            Assert.AreEqual("http://restbucks.com:8080/shop", xml.GetNodeValue("r:shop/@xml:base"));
        }

        [Test]
        public void ShouldNotAddXmlBaseAttributeToRootElementIfBaseUriIsNull()
        {
            var formatter = new ShopFormatter(new ShopBuilder().WithBaseUri(null).Build());
            var xml = new XmlOutput(formatter.CreateXml());

            Assert.IsNull(xml.GetNode("r:shop/@xml:base"));
        }

        private class XmlOutput
        {
            private readonly XPathNavigator navigator;
            private readonly XmlNamespaceManager manager;

            public XmlOutput(XNode element)
            {
                navigator = new XPathDocument(element.CreateReader()).CreateNavigator();
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