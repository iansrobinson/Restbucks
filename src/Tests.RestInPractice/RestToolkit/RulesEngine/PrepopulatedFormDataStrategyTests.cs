using NUnit.Framework;
using Restbucks.RestToolkit.RulesEngine;
using Rhino.Mocks;
using Tests.RestInPractice.RestToolkit.Hacks;
using Tests.RestInPractice.RestToolkit.RulesEngine.Util;

namespace Tests.RestInPractice.RestToolkit.RulesEngine
{
    [TestFixture]
    public class PrepopulatedFormDataStrategyTests
    {
        private static readonly IClientCapabilities ClientCapabilities = CreateClientCapabilities();

        [Test]
        public void ShouldCreateContentBasedOnSuppliedForm()
        {
            var dataStrategy = new PrepopulatedFormDataStrategy(DummyResponse.EntityBody, ExampleMediaType.ContentType);
            var content = dataStrategy.CreateFormData(null, null, ClientCapabilities);

            Assert.AreEqual(DummyResponse.EntityBody.Id, content.ReadAsObject<ExampleEntityBody>(ExampleMediaType.Instance).Id);
        }

        [Test]
        public void ShouldAddContentTypeHeaderToContent()
        {
            var dataStrategy = new PrepopulatedFormDataStrategy(DummyResponse.EntityBody, ExampleMediaType.ContentType);
            var content = dataStrategy.CreateFormData(null, null, ClientCapabilities);

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