using System;
using System.Collections.Generic;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.RestToolkit.RulesEngine;
using Rhino.Mocks;
using Tests.RestInPractice.RestToolkit.Hacks;
using Tests.RestInPractice.RestToolkit.RulesEngine.Util;

namespace Tests.RestInPractice.RestToolkit.RulesEngine
{
    [TestFixture]
    public class ApplicationStateVariablesFormDataStrategyTests
    {
        private static readonly EntityBodyKey Key = new EntityBodyKey("order-form", ExampleMediaType.ContentType, new Uri("http://schemas/shop"));
        private static readonly ApplicationStateVariables StateVariables = new ApplicationStateVariables(new KeyValuePair<IKey, object>(Key, DummyResponse.EntityBody));
        private static readonly IClientCapabilities ClientCapabilities = CreateClientCapabilities();

        [Test]
        public void ShouldRetrieveFormDataFromApplicationContextBasedOnKey()
        {
            var strategy = new ApplicationStateVariablesFormDataStrategy(Key, ExampleMediaType.ContentType);
            var content = strategy.CreateFormData(new HttpResponseMessage(), StateVariables, ClientCapabilities);

            Assert.AreEqual(DummyResponse.EntityBody.Id, content.ReadAsObject<ExampleEntityBody>(ExampleMediaType.Instance).Id);
        }

        [Test]
        public void ShouldAddContentTypeHeaderToContent()
        {
            var strategy = new ApplicationStateVariablesFormDataStrategy(Key, ExampleMediaType.ContentType);
            var content = strategy.CreateFormData(new HttpResponseMessage(), StateVariables, ClientCapabilities);

            Assert.AreEqual(ExampleMediaType.ContentType, content.Headers.ContentType);
        }

        private static IClientCapabilities CreateClientCapabilities()
        {
            var clientCapabilities = MockRepository.GenerateStub<IClientCapabilities>();
            clientCapabilities.Expect(c => c.GetMediaTypeFormatter(ExampleMediaType.ContentType)).Return(ExampleMediaType.Instance);
            return clientCapabilities;
        }
    }
}