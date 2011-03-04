using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.Client.Formatters;
using Restbucks.MediaType;
using Restbucks.NewClient.RulesEngine;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class HttpContentFactoryTests
    {
        private static readonly Shop EntityBody = new ShopBuilder(null).AddItem(new Item("coffee", new Amount("g", 250))).Build();

        [Test]
        public void ShouldCreateHttpContentWithCorrectContentTypeHeader()
        {
            var factory = new HttpContentFactory(RestbucksMediaTypeFormatter.Instance);
            var content = factory.CreateContent(EntityBody, new MediaTypeHeaderValue("application/vnd.restbucks+xml"));

            Assert.AreEqual(new MediaTypeHeaderValue(RestbucksMediaType.Value), content.Headers.ContentType);
            Assert.IsTrue(content.ReadAsString().Length > 0);
        }

        [Test]
        public void ShouldSelectCorrectFormatterFromFormattersWithMultipleSupportedTypes()
        {
            var factory = new HttpContentFactory(new DummyFormatter(), new RestbucksFormatter());
            var content = factory.CreateContent(EntityBody, new MediaTypeHeaderValue("application/vnd.restbucks+xml"));

            Assert.AreEqual(new MediaTypeHeaderValue("application/vnd.restbucks+xml"), content.Headers.ContentType);
            Assert.IsTrue(content.ReadAsString().Length > 0);
        }

        private class RestbucksFormatter : IContentFormatter
        {
            public void WriteToStream(object instance, Stream stream)
            {
                RestbucksMediaTypeFormatter.Instance.WriteToStream(instance, stream);
            }

            public object ReadFromStream(Stream stream)
            {
                return RestbucksMediaTypeFormatter.Instance.ReadFromStream(stream);
            }

            public IEnumerable<MediaTypeHeaderValue> SupportedMediaTypes
            {
                get
                {
                    return new[] {new MediaTypeHeaderValue("application/restbucks+xml")}
                        .Concat(RestbucksMediaTypeFormatter.Instance.SupportedMediaTypes);
                }
            }
        }

        private class DummyFormatter : IContentFormatter
        {
            public void WriteToStream(object instance, Stream stream)
            {
                throw new NotImplementedException();
            }

            public object ReadFromStream(Stream stream)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<MediaTypeHeaderValue> SupportedMediaTypes
            {
                get
                {
                    return new[]
                               {
                                   new MediaTypeHeaderValue("application/xml"),
                                   new MediaTypeHeaderValue("text/html")
                               };
                }
            }
        }
    }
}