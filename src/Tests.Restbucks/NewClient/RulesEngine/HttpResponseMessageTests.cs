using Microsoft.Net.Http;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.NewClient;
using Tests.Restbucks.NewClient.Helpers;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class HttpResponseMessageTests
    {
        [Test]
        public void CanCallReadAsObjectMoreThanOnce()
        {
            var response = StubResponse.CreateResponse();

            var entityBody1 = response.Content.ReadAsObject<Shop>(RestbucksFormatter.Instance);
            var entityBody2 = response.Content.ReadAsObject<Shop>(RestbucksFormatter.Instance);

            Assert.AreEqual(StubResponse.BaseUri, entityBody1.BaseUri);
            Assert.AreEqual(StubResponse.BaseUri, entityBody2.BaseUri);
        }
    }
}