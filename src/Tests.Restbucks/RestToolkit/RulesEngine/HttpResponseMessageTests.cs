using NUnit.Framework;
using Restbucks.Client.Hacks;
using Restbucks.MediaType;
using Tests.Restbucks.Client.Util;

namespace Tests.Restbucks.RestToolkit.RulesEngine
{
    [TestFixture]
    public class HttpResponseMessageTests
    {
        [Test]
        public void CanCallReadAsObjectMoreThanOnce()
        {
            var response = DummyResponse.CreateResponse();

            var entityBody1 = response.Content.ReadAsObject<Shop>(RestbucksMediaTypeFormatter.Instance);
            var entityBody2 = response.Content.ReadAsObject<Shop>(RestbucksMediaTypeFormatter.Instance);

            Assert.AreEqual(DummyResponse.BaseUri, entityBody1.BaseUri);
            Assert.AreEqual(DummyResponse.BaseUri, entityBody2.BaseUri);
        }
    }
}