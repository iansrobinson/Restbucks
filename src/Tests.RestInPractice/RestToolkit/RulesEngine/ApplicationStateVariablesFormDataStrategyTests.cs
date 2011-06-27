using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.RestToolkit.RulesEngine;
using Rhino.Mocks;
using Tests.RestInPractice.RestToolkit.Hacks;
using Tests.RestInPractice.RestToolkit.Utils;

namespace Tests.RestInPractice.RestToolkit.RulesEngine
{
    [TestFixture]
    public class ApplicationStateVariablesFormDataStrategyTests
    {
        private static readonly DummyEntityBody EntityBody = new DummyEntityBody(new Uri("http://localhost/base-uri"), "link-rel", "form-id");
        private static readonly MediaTypeHeaderValue ContentType = DummyMediaType.ContentType;
        private static readonly EntityBodyKey Key = new EntityBodyKey("order-form", ContentType, new Uri("http://schemas/shop"));
        private static readonly ApplicationStateVariables StateVariables = new ApplicationStateVariables(new KeyValuePair<IKey, object>(Key, EntityBody));
        private static readonly IClientCapabilities ClientCapabilities = CreateClientCapabilities();

        [Test]
        public void ShouldRetrieveFormDataFromApplicationContextBasedOnKey()
        {
            var strategy = new ApplicationStateVariablesFormDataStrategy(Key, ContentType);
            var content = strategy.CreateFormData(new HttpResponseMessage(), StateVariables, ClientCapabilities);

            Assert.AreEqual(EntityBody.BaseUri, content.ReadAsObject<DummyEntityBody>(DummyMediaType.Instance).BaseUri);
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
            clientCapabilities.Expect(c => c.GetMediaTypeFormatter(ContentType)).Return(DummyMediaType.Instance);
            return clientCapabilities;
        }
    }
}