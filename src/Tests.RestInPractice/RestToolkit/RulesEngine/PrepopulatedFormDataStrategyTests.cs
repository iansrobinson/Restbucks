using System.Net.Http.Headers;
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
        private static readonly ExampleEntityBody EntityBody = new ExampleEntityBody {Id = 1, Form = new ExampleForm {Id = "form-id", ContentType = "application/xml", Method = "post", Uri = "http://localhost/form"}, Link = new ExampleLink {ContentType = "application/xml", Rel = "rel-value", Uri = "http://localhost/1"}};
        private static readonly MediaTypeHeaderValue ContentType = ExampleMediaType.ContentType;
        private static readonly IClientCapabilities ClientCapabilities = CreateClientCapabilities();

        [Test]
        public void ShouldCreateContentBasedOnSuppliedForm()
        {
            var dataStrategy = new PrepopulatedFormDataStrategy(EntityBody, ContentType);
            var content = dataStrategy.CreateFormData(null, null, ClientCapabilities);

            Assert.AreEqual(EntityBody.Id, content.ReadAsObject<ExampleEntityBody>(ExampleMediaType.Instance).Id);
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
            clientCapabilities.Expect(c => c.GetMediaTypeFormatter(ContentType)).Return(ExampleMediaType.Instance);
            return clientCapabilities;
        }
    }
}