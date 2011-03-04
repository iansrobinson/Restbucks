using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.Client.Formatters;
using Restbucks.MediaType;
using Restbucks.NewClient.RulesEngine;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class HttpContentAdapterTests
    {
        private static readonly Shop EntityBody = CreateEntityBody();
        private static readonly HttpContent Content = CreateHttpContent();

        [Test]
        public void ShouldCreateHttpContentWithCorrectContentTypeHeader()
        {
            var factory = new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance);
            var newContent = factory.CreateContent(EntityBody, new MediaTypeHeaderValue("application/vnd.restbucks+xml"));

            Assert.AreEqual(new MediaTypeHeaderValue(RestbucksMediaType.Value), newContent.Headers.ContentType);
            Assert.IsTrue(newContent.ReadAsString().Length > 0);
        }

        [Test]
        public void ShouldSelectCorrectFormatterFromFormattersWithMultipleSupportedTypes()
        {
            var factory = new HttpContentAdapter(new DummyFormatter(), new RestbucksFormatter());
            var newContent = factory.CreateContent(EntityBody, new MediaTypeHeaderValue("application/vnd.restbucks+xml"));

            Assert.AreEqual(new MediaTypeHeaderValue("application/vnd.restbucks+xml"), newContent.Headers.ContentType);
            Assert.IsTrue(newContent.ReadAsString().Length > 0);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ArgumentException), ExpectedMessage = "Must supply at least one content formatter.\r\nParameter name: formatters")]
        public void ThrowsExceptionIfContentFormattersCollectionIsEmpty()
        {
            new HttpContentAdapter();
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(FormatterNotFoundException), ExpectedMessage = "Formatter not found for content type 'application/vnd.restbucks+xml'.")]
        public void ThrowsExceptionIfNoFormatterExistsForContentType()
        {
            var factory = new HttpContentAdapter(new DummyFormatter());
            factory.CreateContent(EntityBody, new MediaTypeHeaderValue("application/vnd.restbucks+xml"));
        }

        [Test]
        public void ShouldCreateObjectFromContent()
        {
            var factory = new HttpContentAdapter(RestbucksMediaTypeFormatter.Instance);
            var entityBody = (Shop) factory.CreateObject(Content);

            Assert.IsTrue(entityBody.HasItems);
        }

        private static Shop CreateEntityBody()
        {
            return new ShopBuilder(null).AddItem(new Item("coffee", new Amount("g", 250))).Build();
        }

        private static HttpContent CreateHttpContent()
        {
            var content = EntityBody.ToContent(RestbucksMediaTypeFormatter.Instance);
            content.Headers.ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);
            return content;
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