using NUnit.Framework;
using Restbucks.NewClient;
using Restbucks.NewClient.RulesEngine;
using Tests.Restbucks.NewClient.Util;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class HttpResponseMessageExtensionsTests
    {
        [Test]
        public void ShouldReturnTrueIfLinkExists()
        {
            var response = DummyResponse.CreateResponse();
            Assert.IsTrue(response.ContainsLink(RestbucksLink.WithRel("http://relations.restbucks.com/rfq")));
        }

        [Test]
        public void ShouldReturnFalseIfLinkDoesNotExist()
        {
            var response = DummyResponse.CreateResponse();
            Assert.IsFalse(response.ContainsLink(RestbucksLink.WithRel("http://relations.restbucks.com/xyz")));
        }

        [Test]
        public void ShouldReturnTrueIfFormExists()
        {
            var response = DummyResponse.CreateResponse();
            Assert.IsTrue(response.ContainsForm(RestbucksForm.WithId("order-form")));
        }

        [Test]
        public void ShouldReturnFalseIfFormDoesNotExist()
        {
            var response = DummyResponse.CreateResponse();
            Assert.IsFalse(response.ContainsForm(RestbucksForm.WithId("xyz")));
        }
    }
}