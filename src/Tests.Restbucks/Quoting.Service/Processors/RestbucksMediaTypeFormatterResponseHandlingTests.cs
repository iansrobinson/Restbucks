using System;
using System.IO;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Quoting.Service.Processors;

namespace Tests.Restbucks.Quoting.Service.Processors
{
    [TestFixture]
    public class RestbucksMediaTypeFormatterResponseHandlingTests
    {
        [Test]
        public void ShouldSupportRestbucksPlusXmlAndApplicationXmlAndTextXmlMediaTypes()
        {
            var processor = new RestbucksMediaTypeFormatter();

            Assert.AreEqual(3, processor.SupportedMediaTypes.Count());
            Assert.AreEqual(RestbucksMediaType.Value, processor.SupportedMediaTypes.First().MediaType);
            Assert.AreEqual("application/xml", processor.SupportedMediaTypes.Skip(1).First().MediaType);
            Assert.AreEqual("text/xml", processor.SupportedMediaTypes.Skip(2).First().MediaType);
        }

        [Test]
        public void ShouldWriteShopToStream()
        {
            const string expectedXml = @"<?xml version=""1.0"" encoding=""utf-8""?><shop xml:base=""http://restbucks.com/"" xmlns=""http://schemas.restbucks.com/shop"" />";

            var shop = new ShopBuilder(new Uri("http://restbucks.com/")).Build();
            var stream = new MemoryStream();

            var processor = new RestbucksMediaTypeFormatter();
            processor.WriteToStream(typeof (Shop), shop, stream, null, null);

            stream.Seek(0, SeekOrigin.Begin);

            var xml = new XmlDocument();
            xml.Load(stream);

            Assert.AreEqual(expectedXml, xml.OuterXml);
        }

        private static void PrintStream(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(stream);
            Console.WriteLine(reader.ReadToEnd());
        }
    }
}