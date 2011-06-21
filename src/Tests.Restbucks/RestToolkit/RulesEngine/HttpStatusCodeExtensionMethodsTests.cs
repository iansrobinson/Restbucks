using System.Net;
using NUnit.Framework;
using Restbucks.RestToolkit.RulesEngine;

namespace Tests.Restbucks.RestToolkit.RulesEngine
{
    [TestFixture]
    public class HttpStatusCodeExtensionMethodsTests
    {
        [Test]
        public void Is1XXTests()
        {
            Assert.IsTrue(HttpStatusCode.Continue.Is1XX());
            Assert.IsTrue(HttpStatusCode.SwitchingProtocols.Is1XX());

            Assert.IsFalse(HttpStatusCode.OK.Is1XX());
        }

        [Test]
        public void Is2XXTests()
        {
            Assert.IsTrue(HttpStatusCode.OK.Is2XX());
            Assert.IsTrue(HttpStatusCode.Created.Is2XX());

            Assert.IsFalse(HttpStatusCode.Continue.Is2XX());
            Assert.IsFalse(HttpStatusCode.SeeOther.Is2XX());
        }

        [Test]
        public void Is3XXTests()
        {
            Assert.IsTrue(HttpStatusCode.MultipleChoices.Is3XX());
            Assert.IsTrue(HttpStatusCode.TemporaryRedirect.Is3XX());

            Assert.IsFalse(HttpStatusCode.OK.Is3XX());
            Assert.IsFalse(HttpStatusCode.NotFound.Is3XX());
        }

        [Test]
        public void Is4XXTests()
        {
            Assert.IsTrue(HttpStatusCode.BadRequest.Is4XX());
            Assert.IsTrue(HttpStatusCode.ExpectationFailed.Is4XX());

            Assert.IsFalse(HttpStatusCode.MultipleChoices.Is4XX());
            Assert.IsFalse(HttpStatusCode.InternalServerError.Is4XX());
        }

        [Test]
        public void Is5XXTests()
        {
            Assert.IsTrue(HttpStatusCode.InternalServerError.Is5XX());
            Assert.IsTrue(HttpStatusCode.HttpVersionNotSupported.Is5XX());

            Assert.IsFalse(HttpStatusCode.BadRequest.Is5XX());
        }
    }
}