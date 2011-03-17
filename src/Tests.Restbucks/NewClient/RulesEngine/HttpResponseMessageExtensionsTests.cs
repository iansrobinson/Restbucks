using System.Net.Http;
using NUnit.Framework;
using Restbucks.NewClient;
using Restbucks.NewClient.RulesEngine;
using Tests.Restbucks.NewClient.Util;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class HttpResponseMessageExtensionsTests
    {
        private static readonly HttpResponseMessage Response = DummyResponse.CreateResponse();
        
        [Test]
        public void ShouldReturnTrueIfLinkExists()
        {
            Assert.IsTrue(Response.ContainsLink(RestbucksLink.WithRel("http://relations.restbucks.com/rfq")));
        }

        [Test]
        public void ShouldReturnFalseIfLinkDoesNotExist()
        {
            Assert.IsFalse(Response.ContainsLink(RestbucksLink.WithRel("http://relations.restbucks.com/xyz")));
        }

        [Test]
        public void ShouldReturnTrueIfFormExists()
        {
            Assert.IsTrue(Response.ContainsForm(RestbucksForm.WithId("request-for-quote")));
        }

        [Test]
        public void ShouldReturnFalseIfFormDoesNotExist()
        {
            Assert.IsFalse(Response.ContainsForm(RestbucksForm.WithId("xyz")));
        }
    }
}