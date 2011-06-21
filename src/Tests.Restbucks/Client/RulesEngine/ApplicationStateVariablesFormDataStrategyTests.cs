using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.Client;
using Restbucks.Client.RulesEngine;
using Rhino.Mocks;

namespace Tests.Restbucks.Client.RulesEngine
{
    [TestFixture]
    public class ApplicationStateVariablesFormDataStrategyTests
    {
        private static readonly Shop EntityBody = new ShopBuilder(new Uri("http://localhost/restbucks/")).Build();
        private static readonly MediaTypeHeaderValue ContentType = new MediaTypeHeaderValue(RestbucksMediaType.Value);
        private static readonly EntityBodyKey Key = new EntityBodyKey("order-form", ContentType, new Uri("http://schemas/shop"));
        private static readonly ApplicationStateVariables StateVariables = new ApplicationStateVariables(new KeyValuePair<IKey, object>(Key, EntityBody));
        private static readonly IClientCapabilities ClientCapabilities = CreateClientCapabilities();

        [Test]
        public void ShouldRetrieveFormDataFromApplicationContextBasedOnKey()
        {
            var strategy = new ApplicationStateVariablesFormDataStrategy(Key, ContentType);
            var content = strategy.CreateFormData(new HttpResponseMessage(), StateVariables, ClientCapabilities);

            Assert.AreEqual(EntityBody.BaseUri, content.ReadAsObject<Shop>(RestbucksFormatter.Instance).BaseUri);
        }

        [Test]
        public void ShouldAddContentTypeHeaderToContent()
        {
            var strategy = new ApplicationStateVariablesFormDataStrategy(Key, ContentType);
            var content = strategy.CreateFormData(new HttpResponseMessage(), StateVariables, ClientCapabilities);

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