using System.Net.Http;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.RulesEngine;
using Tests.Restbucks.Client.Util;

namespace Tests.Restbucks.Client.RulesEngine
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