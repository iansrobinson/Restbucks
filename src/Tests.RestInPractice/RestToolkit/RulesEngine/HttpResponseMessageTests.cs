using NUnit.Framework;
using Tests.RestInPractice.RestToolkit.Hacks;
using Tests.RestInPractice.RestToolkit.RulesEngine.Util;
using Tests.RestInPractice.RestToolkit.Utils;

namespace Tests.RestInPractice.RestToolkit.RulesEngine
{
    [TestFixture]
    public class HttpResponseMessageTests
    {
        [Test]
        public void CanCallReadAsObjectMoreThanOnce()
        {
            var response = DummyResponse.CreateResponse();

            var entityBody1 = response.Content.ReadAsObject<DummyEntityBody>(DummyMediaType.Instance);
            var entityBody2 = response.Content.ReadAsObject<DummyEntityBody>(DummyMediaType.Instance);

            Assert.AreEqual(DummyResponse.BaseUri, entityBody1.BaseUri);
            Assert.AreEqual(DummyResponse.BaseUri, entityBody2.BaseUri);
        }
    }
}