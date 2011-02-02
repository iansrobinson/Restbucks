using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Description;
using System.Xml;
using Microsoft.ServiceModel.Http;
using NUnit.Framework;
using Restbucks.Quoting.Service.Processors;
using Tests.Restbucks.MediaType;
using Tests.Restbucks.MediaType.Helpers;

namespace Tests.Restbucks.Quoting.Service.Processors
{
    [TestFixture]
    public class RestbucksMediaTypeProcessorResponseHandlingTests
    {
        [Test]
        public void ShouldSupportRestbucksPlusXmlAndApplicationXmlAndTextXmlMediaTypes()
        {
            var processor = new RestbucksMediaTypeProcessor(new HttpOperationDescription(), MediaTypeProcessorMode.Response);

            Assert.AreEqual(3, processor.SupportedMediaTypes.Count());
            Assert.AreEqual("application/restbucks+xml", processor.SupportedMediaTypes.First());
            Assert.AreEqual("application/xml", processor.SupportedMediaTypes.Skip(1).First());
            Assert.AreEqual("text/xml", processor.SupportedMediaTypes.Skip(2).First());
        }

        [Test]
        public void ShouldWriteShopToStream()
        {
            const string expectedXml = @"<?xml version=""1.0"" encoding=""utf-8""?><shop xml:base=""http://restbucks.com/"" xmlns=""http://schemas.restbucks.com/shop"" />";
            
            var shop = new ShopBuilder().WithBaseUri(new Uri("http://restbucks.com/")).Build();
            var stream = new MemoryStream();

            var processor = new RestbucksMediaTypeProcessor(new HttpOperationDescription(), MediaTypeProcessorMode.Response);
            processor.WriteToStream(shop, stream, new HttpRequestMessage());

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