using System;
using System.Net.Http.Headers;
using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.NewClient;
using Restbucks.NewClient.RulesEngine;
using Rhino.Mocks;

namespace Tests.Restbucks.NewClient.RulesEngine
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
            clientCapabilities.Expect(c => c.GetContentFormatter(ContentType)).Return(RestbucksFormatter.Instance);
            return clientCapabilities;
        }
    }
}