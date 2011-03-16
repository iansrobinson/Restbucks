using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.NewClient;
using Tests.Restbucks.NewClient.Util;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class HttpResponseMessageTests
    {
        [Test]
        public void CanCallReadAsObjectMoreThanOnce()
        {
            var response = DummyResponse.CreateResponse();

            var entityBody1 = response.Content.ReadAsObject<Shop>(RestbucksFormatter.Instance);
            var entityBody2 = response.Content.ReadAsObject<Shop>(RestbucksFormatter.Instance);

            Assert.AreEqual(DummyResponse.BaseUri, entityBody1.BaseUri);
            Assert.AreEqual(DummyResponse.BaseUri, entityBody2.BaseUri);
        }
    }
}