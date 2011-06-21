using System;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Client;
using Restbucks.Client.RulesEngine;
using Rhino.Mocks;

namespace Tests.Restbucks.Client.RulesEngine
{
    [TestFixture]
    public class PrepopulatedFormDataStrategyTests
    {
        private static readonly Shop EntityBody = new ShopBuilder(new Uri("http://restbucks.com")).Build();
        private static readonly MediaTypeHeaderValue ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);
        private static readonly IClientCapabilities ClientCapabilities = CreateClientCapabilities();

        [Test]
        public void ShouldCreateContentBasedOnSuppliedForm()
        {
            var dataStrategy = new PrepopulatedFormDataStrategy(EntityBody, ContentType);
            var content = dataStrategy.CreateFormData(null, null, ClientCapabilities);

            Assert.AreEqual(EntityBody.BaseUri, content.ReadAsObject<Shop>(RestbucksFormatter.Instance).BaseUri);
        }

        [Test]
        public void ShouldAddContentTypeHeaderToContent()
        {
            var dataStrategy = new PrepopulatedFormDataStrategy(EntityBody, ContentType);
            var content = dataStrategy.CreateFormData(null, null, ClientCapabilities);

            Assert.AreEqual(ContentType, content.Headers.ContentType);
        }

        private static IClientCapabilities CreateClientCapabilities()
        {
            var clientCapabilities = MockRepository.GenerateStub<IClientCapabilities>();
            clientCapabilities.Expect(c => c.GetMediaTypeFormatter(ContentType)).Return(RestbucksFormatter.Instance);
            return clientCapabilities;
        }
    }
}