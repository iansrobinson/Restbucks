using NUnit.Framework;
using Tests.RestInPractice.RestToolkit.Hacks;
using Tests.RestInPractice.RestToolkit.RulesEngine.Util;

namespace Tests.RestInPractice.RestToolkit.RulesEngine
{
    [TestFixture]
    public class HttpResponseMessageTests
    {
        [Test]
        public void CanCallReadAsObjectMoreThanOnce()
        {
            var response = DummyResponse.CreateResponse();

            var entityBody1 = response.Content.ReadAsObject<ExampleEntityBody>(ExampleMediaType.Instance);
            var entityBody2 = response.Content.ReadAsObject<ExampleEntityBody>(ExampleMediaType.Instance);

            Assert.AreEqual(DummyResponse.EntityBody.Id, entityBody1.Id);
            Assert.AreEqual(DummyResponse.EntityBody.Id, entityBody2.Id);
        }
    }
}