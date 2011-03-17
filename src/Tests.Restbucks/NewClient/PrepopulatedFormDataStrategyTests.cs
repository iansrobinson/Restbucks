using System;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.NewClient;

namespace Tests.Restbucks.NewClient
{
    [TestFixture]
    public class PrepopulatedFormDataStrategyTests
    {
        private static readonly Shop EntityBody = new ShopBuilder(new Uri("http://restbucks.com")).Build();
        private static readonly MediaTypeHeaderValue ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);

        [Test]
        public void ShouldCreateContentBasedOnSuppliedForm()
        {
            var dataStrategy = new PrepopulatedFormDataStrategy(EntityBody, ContentType);
            var content = dataStrategy.CreateFormData(null, null);

            Assert.AreEqual(EntityBody.BaseUri, content.ReadAsObject<Shop>(RestbucksFormatter.Instance).BaseUri);
        }

        [Test]
        public void ShouldAddContentTypeHeaderToContent()
        {
            var dataStrategy = new PrepopulatedFormDataStrategy(EntityBody, ContentType);
            var content = dataStrategy.CreateFormData(null, null);

            Assert.AreEqual(ContentType, content.Headers.ContentType);
        }
    }
}