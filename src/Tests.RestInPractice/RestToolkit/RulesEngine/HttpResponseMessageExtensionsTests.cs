using System.Net.Http;
using NUnit.Framework;
using RestInPractice.RestToolkit.RulesEngine;
using Tests.RestInPractice.RestToolkit.RulesEngine.Util;

namespace Tests.RestInPractice.RestToolkit.RulesEngine
{
    [TestFixture]
    public class HttpResponseMessageExtensionsTests
    {
        private static readonly HttpResponseMessage Response = DummyResponse.CreateResponse();

        [Test]
        public void ShouldReturnTrueIfLinkExists()
        {
            Assert.IsTrue(Response.ContainsLink(new Link(DummyResponse.Link.Rel)));
        }

        [Test]
        public void ShouldReturnFalseIfLinkDoesNotExist()
        {
            Assert.IsFalse(Response.ContainsLink(new Link("does-not-exist")));
        }

        [Test]
        public void ShouldReturnTrueIfFormExists()
        {
            Assert.IsTrue(Response.ContainsForm(new Form(DummyResponse.Form.Id)));
        }

        [Test]
        public void ShouldReturnFalseIfFormDoesNotExist()
        {
            Assert.IsFalse(Response.ContainsForm(new Form("does-not-exist")));
        }
    }
}